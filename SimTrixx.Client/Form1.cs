using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SimTrixx.Reader.Concrete;
using SimTrixx.Reader.Concrete.Enums;
using System.IO;
using AutoUpdaterDotNET;
using SimTrixx.Data.Repos;
using SimTrixx.Client.Logic;
using System.Diagnostics;
namespace TestDocReader
{
    public partial class Form1 : Form
    {
        private string tempfile = Path.GetTempFileName();
        private string runTimePath = Environment.CurrentDirectory;
        private string _currentDocument;
        private List<Contract> _documentLines;
        private List<Word> _keywords;
        private  System.Timers.Timer loadingTimer;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            
            InitializeComponent();
            tmrLoading.Start();
            CheckUpdate();
            CheckLicense();
            cmbFilter.SelectedIndex = (int)Properties.Settings.Default["FilterType"];
            cbSectionFilter.Checked = (bool)Properties.Settings.Default["AdvSectionFilter"];
            _documentLines = new List<Contract>();
            _keywords = new List<Word>();
            LoadKeywords();

            LoadHtmlGrid();

            //var url = $@"{runTimePath}\Configs\ContractDataViewer.html";
            var url = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "ContractDataViewer.html");
            webBrowser1.Url = new Uri(url);
            //Type dgvType =dataGridView1.GetType();
            //var pi = dgvType.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //pi.SetValue(dataGridView1, new object[]{true}, null);

        }





        public void CheckLicense()
        {

            var licenseKey = Properties.Settings.Default["License"];
            if(!string.IsNullOrEmpty(licenseKey.ToString())) 
            {
                var license = new LicenseManager().CheckLicense(licenseKey.ToString());
                if (license)
                {
                    EnableForm();
                } else
                {
                    MessageBox.Show("Your license is not valid, please contact support.", "License", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    DisableForm();
                }
            } else
            {
                MessageBox.Show("Please enter you license key in the settings menu and restart the app.", "License", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DisableForm();
            }
        }

        private void DisableForm()
        {
            btnImport.Enabled = false;
            btnOutput.Enabled = false;
        }

        private void EnableForm()
        {
            btnImport.Enabled = true;
            btnOutput.Enabled = true;
        }

        private void CheckUpdate()
        {
            AutoUpdater.Start("https://simtrixx.blob.core.windows.net/install/Update.xml");
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            _documentLines = null;
            var strBuilder = new System.Text.StringBuilder();
            var msbResult = new frmMessageBox().ShowDialog();

            if (msbResult == DialogResult.OK)
            {
                var result = ofdDocument.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LoadKeywords();
                    if (!string.IsNullOrWhiteSpace(ofdDocument.FileName))
                    {
                        _currentDocument = ofdDocument.FileName;
                    }
                    ImportDocument();
                }
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            if (_documentLines != null)
            {
                try
                {

                    var exportHandler = new Logic.FileExportHandler();
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Word Document |*.docx|Legacy Word Doc|*.doc|Excel|*.xlsx";
                    saveFileDialog1.DefaultExt = ".docx";
                    saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        if (saveFileDialog1.FilterIndex == 1)
                        {
                            exportHandler.CreateWordDoc(_documentLines, saveFileDialog1.FileName);
                        }
                        else if (saveFileDialog1.FilterIndex == 2)
                        {
                           var doc = exportHandler.LinesToDoc(_documentLines);
                            System.IO.File.AppendAllText(saveFileDialog1.FileName, doc);
                        }
                        else if (saveFileDialog1.FilterIndex == 3)
                        {
                            exportHandler.CreateExcelDoc(_documentLines, saveFileDialog1.FileName);
                        }
                        Cursor.Current = Cursors.Default;
                        MessageBox.Show($@"Export complete {Environment.NewLine} File saved to {saveFileDialog1.FileName}");
                    }
                    else
                    {
                        MessageBox.Show($@"Export operation has been canceled");
                    }
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show(@"Please load a document first");
            }
        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            var frm = new frmSettings(this);
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        private void LoadKeywords()
        {
            _keywords = new Logic.KeywordConfigHandler().ImportV2().Keywords;
        }

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnCloseFrm_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void LoadHtmlGrid()
        {
            var doc = new Logic.GridDataHandler().BuildHtmlString(_documentLines);
            var htmlDocPath = string.Empty;
            htmlDocPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "ContractDataViewer.html");

            //System.IO.File.WriteAllText($@"{runTimePath}\Configs\ContractDataViewer.html", doc);
            System.IO.File.WriteAllText(htmlDocPath,doc);

            webBrowser1.Refresh();
        }

        private void ImportDocument()
        {
            if (!string.IsNullOrWhiteSpace(_currentDocument))
            {

                //*****************************Uncomment for V2*****************************************
                //var fileType = new Logic.FileExtensionHandler().GetDocumentType(_currentDocument);
                //var contract = new ContractReaderV2.ReaderV2(_currentDocument, tempfile);

                //switch (fileType)
                //{
                //    case Logic.FileExtensionHandler.FileType.WordDoc:
                //        _documentLines = contract.ParseWordDocument(_keywords);
                //        break;
                //    case Logic.FileExtensionHandler.FileType.Pdf:
                //        _documentLines = contract.ParsePdfDocument(_keywords);
                //        break;
                //    default:
                //        MessageBox.Show($@"Error => Error occured while deriving file type{Environment.NewLine}Either file was not found or the file type was unsupported", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        _documentLines = null;
                //        break;
                //}
                //***************************************************************************************



                //**************************V3***********************************************************

                var reader = new ContractReaderV2.DocumentManager(_currentDocument, tempfile);
                var advancedFiltering = cbSectionFilter.Checked;
                if (cmbFilter.SelectedIndex == 0)
                {
                    _documentLines = reader.ParseDocument(_keywords, GlobalEnum.DocumentParseMode.KeyWordSectionsWithSplits, advancedFiltering);
                }
                if (cmbFilter.SelectedIndex == 1)
                {
                    _documentLines = reader.ParseDocument(_keywords, GlobalEnum.DocumentParseMode.KeyWordSectionsOnly, advancedFiltering);
                }
                if (cmbFilter.SelectedIndex == 2)
                {
                    _documentLines = reader.ParseDocument(_keywords, GlobalEnum.DocumentParseMode.FullDocument, advancedFiltering);
                }



                //**************************V3***********************************************************





                if (_documentLines == null)
                {
                    MessageBox.Show($@"Error => Error opening document{Environment.NewLine}Please make sure the document is not already open.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    LoadHtmlGrid();
                    //Write to HTML here
                    //_documentlines

                    label2.Text = _currentDocument;
                }
            }
        }

        #region Menu MouseOver
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pnlImport_MouseEnter(object sender, EventArgs e)
        {
            pnlImport.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pnlImport_MouseLeave(object sender, EventArgs e)
        {
            pnlImport.BorderStyle = BorderStyle.None;
        }

        private void btnLoadDocument_MouseEnter(object sender, EventArgs e)
        {
            pnlImport.BorderStyle = BorderStyle.FixedSingle;
            //btnLoadDocument.BackColor = Color.FromArgb(37, 46, 59);
        }

        private void btnLoadDocument_MouseLeave(object sender, EventArgs e)
        {
            pnlImport.BorderStyle = BorderStyle.None;
        }

        private void pnlExport_MouseEnter(object sender, EventArgs e)
        {
            pnlExport.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pnlExport_MouseLeave(object sender, EventArgs e)
        {
            pnlExport.BorderStyle = BorderStyle.None;
        }

        private void btnOutput_MouseEnter(object sender, EventArgs e)
        {
            pnlExport.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btnOutput_MouseLeave(object sender, EventArgs e)
        {
            pnlExport.BorderStyle = BorderStyle.None;
        }
        private void pnlSettings_MouseEnter(object sender, EventArgs e)
        {
            pnlSettings.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pnlSettings_MouseLeave(object sender, EventArgs e)
        {
            pnlSettings.BorderStyle = BorderStyle.None;
        }

        private void btnSettings_MouseEnter(object sender, EventArgs e)
        {
            pnlSettings.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btnSettings_MouseLeave(object sender, EventArgs e)
        {
            pnlSettings.BorderStyle = BorderStyle.None;
        }


        #endregion

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //var searchString = "shall";
            //if (e.RowIndex > -1 && e.ColumnIndex > -1 && dataGridView1.Columns[e.ColumnIndex].Name != "Section")
            //{
            //    // Check data for search  
     
            //        String gridCellValue = e.FormattedValue.ToString();
            //        // check the index of search text into grid cell.  
            //        int startIndexInCellValue = gridCellValue.ToLower().IndexOf(searchString.Trim().ToLower());
            //        // IF search text is exists inside grid cell then startIndexInCellValue value will be greater then 0 or equal to 0  
            //        if (startIndexInCellValue >= 0)
            //        {
            //            e.Handled = true;
            //            e.PaintBackground(e.CellBounds, true);
            //            //the highlite rectangle  
            //            Rectangle hl_rect = new Rectangle();
            //            hl_rect.Y = e.CellBounds.Y + 2;
            //            hl_rect.Height = 10;//e.CellBounds.Height - 20;
            //            //find the size of the text before the search word in grid cell data.  
            //            String sBeforeSearchword = gridCellValue.Substring(0, startIndexInCellValue);
            //            //size of the search word in the grid cell data  
            //            String sSearchWord = gridCellValue.Substring(startIndexInCellValue, searchString.Trim().Length);
            //            Size s1 = TextRenderer.MeasureText(e.Graphics, sBeforeSearchword, e.CellStyle.Font, e.CellBounds.Size);
            //            Size s2 = TextRenderer.MeasureText(e.Graphics, sSearchWord, e.CellStyle.Font, e.CellBounds.Size);
            //            if (s1.Width > 5)
            //            {
            //                hl_rect.X = e.CellBounds.X + s1.Width - 5;
            //                hl_rect.Width = s2.Width - 6;
            //            }
            //            else
            //            {
            //                hl_rect.X = e.CellBounds.X + 2;
            //                hl_rect.Width = s2.Width - 6;
            //            }
            //            //color for showing highlighted text in grid cell  
            //            SolidBrush hl_brush;
            //            hl_brush = new SolidBrush(Color.Yellow);
            //            //paint the background behind the search word  
            //            e.Graphics.FillRectangle(hl_brush, hl_rect);
            //            hl_brush.Dispose();
            //            e.PaintContent(e.CellBounds);
            //        }
                
            //}
        }

        private void tmrLoading_Tick(object sender, EventArgs e)
        {
            tmrLoading.Stop();
            pnlLoading.Dispose();
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["FilterType"] = cmbFilter.SelectedIndex;
            ImportDocument();
        }

        private void cbSectionFilter_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["AdvSectionFilter"] = cbSectionFilter.Checked;
            ImportDocument();
        }

        private void CreateDocumentDirectory()
        {

            if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrix")))
            {

            }
        }
    }
}
