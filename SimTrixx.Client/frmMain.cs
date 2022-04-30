using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SimTrixx.Reader.Concrete;
using SimTrixx.Reader.Concrete.Enums;
using System.IO;
using AutoUpdaterDotNET;
using System.Reflection;
using SimTrixx.Client.Logic;
using SimTrixx.Common.Enums;

namespace SimTrixx.Client
{
    public partial class frmMain : Form
    {
        private readonly string _tempfile = Path.GetTempFileName();
        private string _currentDocument;
        private List<Contract> _documentLines;
        private BindingList<Word> _keywords;
        
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private readonly LoggingHandler _log = new LoggingHandler($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Simtrixx\");

        public frmMain()
        {
            InitializeComponent();
            Cursor.Current = Cursors.WaitCursor;
            lblVersion.Text = $@"Initializing...{Environment.NewLine}Version {Assembly.GetExecutingAssembly().GetName().Version}";
            tmrLoading.Start();
            CheckUpdate();
            CheckLicense();
            cmbFilter.SelectedIndex = 0;
            cbSectionFilter.Checked = false;
            _documentLines = new List<Contract>();
            _keywords = new BindingList<Word>();
            LoadKeywords();

            LoadHtmlGrid();
            
            var url = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "ContractDataViewer.html");
            webBrowser1.Url = new Uri(url);
            Cursor.Current = Cursors.Default;
        }

        public void CheckLicense()
        {

            var licenseKey = new LicenseHandler().GetLicense();// Properties.Settings.Default["License"];
            if(!string.IsNullOrEmpty(licenseKey.Key)) 
            {
                var license = new LicenseHandler().CheckLicense(licenseKey.Key);
                if (license)
                {
                    _log.LogIt(LogType.Information, $"License Verified {licenseKey}");
                    EnableForm();
                } else
                {
                    _log.LogIt(LogType.Error, $"Your license is not valid, please contact support. {licenseKey}");
                    MessageBox.Show(@"Your license is not valid, please contact support.", @"License", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    DisableForm();
                }
            } else
            {
                _log.LogIt(LogType.Error, $"No License Key Present");
                MessageBox.Show(@"Please enter you license key in the settings menu and restart the app.", @"License", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DisableForm();
            }
        }

        private void DisableForm()
        {
            btnImport.Enabled = false;
            btnOutput.Enabled = false;
            btnExportAbbrv.Enabled = false;
        }

        private void EnableForm()
        {
            btnImport.Enabled = true;
            btnOutput.Enabled = true;
            btnExportAbbrv.Enabled = true;
        }

        private void CheckUpdate()
        {
            _log.LogIt(LogType.Information, $"Checking For Update");
            AutoUpdater.Start("https://simtrixx.blob.core.windows.net/install/Update.xml");
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            _documentLines = null;
            var msbResult = new frmMessageBox().ShowDialog();

            if (msbResult != DialogResult.OK) return;
            var result = ofdDocument.ShowDialog();
            if (result != DialogResult.OK) return;
            LoadKeywords();
            if (!string.IsNullOrWhiteSpace(ofdDocument.FileName))
            {
                _currentDocument = ofdDocument.FileName;
            }
            ImportDocument();
        }
        
        private void btnOutput_Click(object sender, EventArgs e)
        {
            if (_documentLines != null)
            {
                try
                {
                    _log.LogIt(LogType.Information, $"Exporting Processes Initiated");
                    var exportHandler = new Logic.FileExportHandler();
                    
                    var saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = @"Word Document |*.docx|Legacy Word Doc|*.doc|Excel|*.xlsx";
                    saveFileDialog1.DefaultExt = ".docx";
                    saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        switch (saveFileDialog1.FilterIndex)
                        {
                            case 1:
                                exportHandler.CreateWordDoc(_documentLines, saveFileDialog1.FileName);
                                break;
                            case 2:
                            {
                                var doc = exportHandler.LinesToDoc(_documentLines);
                                File.AppendAllText(saveFileDialog1.FileName, doc);
                                break;
                            }
                            case 3:
                                exportHandler.CreateExcelDoc(_documentLines, saveFileDialog1.FileName);
                                break;
                        }
                        Cursor.Current = Cursors.Default;
                        _log.LogIt(LogType.Information, $"Exporting Complete, File Saved to {saveFileDialog1.FileName}");
                        MessageBox.Show($@"File saved to {saveFileDialog1.FileName}", @"Export complete");
                    }
                    else
                    {
                        _log.LogIt(LogType.Information, $"Exporting Processes Cancelled");
                        MessageBox.Show($@"Export operation has been cancelled");
                    }
                    
                }
                catch(Exception ex)
                {
                    _log.LogIt(LogType.Error, $"Error while exporting {Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.InnerException}{Environment.NewLine}{ex.StackTrace}");
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                _log.LogIt(LogType.Information, "Output clicked prior to importing.");
                MessageBox.Show(@"Please load a document first");
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var frm = new frmSettings(this);
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }

        private void btnExportAbbrv_Click(object sender, EventArgs e)
        {
            _documentLines = null;
            var exportHandler = new FileExportHandler();
            LoadKeywords();
            try
            {
                var result = ofdDocument.ShowDialog();
                if (result != DialogResult.OK) return;
                LoadKeywords();
                if (!string.IsNullOrWhiteSpace(ofdDocument.FileName))
                {
                    _currentDocument = ofdDocument.FileName;
                }
                var reader = new ContractReaderV2.DocumentManager(_currentDocument, _tempfile);
                var abbrvList = reader.GetAbbriviations();
                var saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = @"Excel|*.xlsx";
                saveFileDialog1.DefaultExt = ".xlsx";
                saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    exportHandler.CreateExcelAbbrviationDoc(abbrvList, saveFileDialog1.FileName);
                    _log.LogIt(LogType.Information, $"Export Complete {saveFileDialog1.FileName}");
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show($@"File saved to {saveFileDialog1.FileName}", @"Export complete");
                }
                else
                {
                    _log.LogIt(LogType.Information, "Export Cancelled");
                    MessageBox.Show(@"Export operation has been canceled");
                }
            }
            catch (Exception ex)
            {
                _log.LogIt(LogType.Error, $"Error while exporting {Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.InnerException}{Environment.NewLine}{ex.StackTrace}");
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadKeywords()
        {
            _keywords = new KeywordConfigHandler().ImportV2().Keywords;
        }

        private void frmMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void btnCloseFrm_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void LoadHtmlGrid()
        {
            var doc = new Logic.GridDataHandler().BuildHtmlString(_documentLines);
            var htmlDocPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "ContractDataViewer.html");
            
            File.WriteAllText(htmlDocPath,doc);

            webBrowser1.Refresh();
        }
        private void ImportDocument()
        {
            _log.LogIt(LogType.Information, "Importing Document");
            if (string.IsNullOrWhiteSpace(_currentDocument)) return;
            var reader = new ContractReaderV2.DocumentManager(_currentDocument, _tempfile);
            var advancedFiltering = cbSectionFilter.Checked;
            switch (cmbFilter.SelectedIndex)
            {
                case 0:
                    _documentLines = reader.ParseDocument(_keywords, GlobalEnum.DocumentParseMode.KeyWordSectionsWithSplits, advancedFiltering);
                    break;
                case 1:
                    _documentLines = reader.ParseDocument(_keywords, GlobalEnum.DocumentParseMode.KeyWordSectionsOnly, advancedFiltering);
                    break;
                case 2:
                    _documentLines = reader.ParseDocument(_keywords, GlobalEnum.DocumentParseMode.FullDocument, advancedFiltering);
                    break;
            }

            if (_documentLines == null)
            {
                _log.LogIt(LogType.Error, $"Error opening document{Environment.NewLine}Please make sure the document is not already open.");
                MessageBox.Show($@"Error => Error opening document{Environment.NewLine}Please make sure the document is not already open.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Cursor.Current = Cursors.WaitCursor;
                LoadHtmlGrid();
                Cursor.Current = Cursors.Default;
                //Write to HTML here
                label2.Text = _currentDocument;
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
        private void pnlAcronym_MouseEnter(object sender, EventArgs e)
        {
            pnlAcronym.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pnlAcronym_MouseLeave(object sender, EventArgs e)
        {
            pnlAcronym.BorderStyle = BorderStyle.None;
        }

        private void btnExportAbbrv_MouseEnter(object sender, EventArgs e)
        {
            pnlAcronym.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btnExportAbbrv_MouseLeave(object sender, EventArgs e)
        {
            pnlAcronym.BorderStyle = BorderStyle.None;
        }


        #endregion

        //private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        //{
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
        //}

        private void tmrLoading_Tick(object sender, EventArgs e)
        {
            tmrLoading.Stop();
            pnlLoading.Dispose();
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImportDocument();
        }

        private void cbSectionFilter_CheckedChanged(object sender, EventArgs e)
        {
            ImportDocument();
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }
    }
}
