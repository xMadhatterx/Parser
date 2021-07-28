using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ContractReaderV2.Concrete;
using Portable.Licensing;

namespace TestDocReader
{
    public partial class Form1 : Form
    {
        private readonly string _documentPath = System.Configuration.ConfigurationManager.AppSettings.Get("DocumentPath");
        private const string TemporaryFileName = "Temp_Output.txt";
        private readonly string _outputDocumentPath = $"{System.Configuration.ConfigurationManager.AppSettings.Get("OutputDocumentPath")}output.html";
        private string _currentDocument;
        private List<Contract> _documentLines;
        private List<Word> _keywords;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
            //CheckLicense();
            _documentLines = new List<Contract>();
            _keywords = new List<Word>();
            LoadKeywords();
        }

        private void CheckLicense()
        {
            //var license = License.Load();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            var strBuilder = new System.Text.StringBuilder();
            //strBuilder.Append("Please remove all document headers and footers \r \r");
            //strBuilder.Append("Removing PDF headers and footers \r");
            //strBuilder.Append("\t1.Open the edit navigation bar \r");
            //strBuilder.Append("\t2.Click on 'Pull-Down' and select 'Remove'\r");
            //strBuilder.Append("\t3.Click 'Yes' when prompted, then SAVE document\r");
            //strBuilder.Append("\t4.Import your PDF document with headers and footers removed\r");
            //strBuilder.Append("For help with removing headers and footers with Microsoft Word please see: \r");
            //strBuilder.Append("https://support.microsoft.com/en-us/office/remove-all-headers-and-footers-953e158d-425d-47b0-bf56-b02cb34772aa \r");
            //strBuilder.Append("For help with removing headers and footers with Adobe please see (half way down the page): \r");
            //strBuilder.Append("https://helpx.adobe.com/acrobat/using/add-headers-footers-pdfs.html");
            //var msbResult = MessageBox.Show(strBuilder.ToString(), "Please remove all document headers and footers", MessageBoxButtons.OKCancel);
            var msbResult = new frmMessageBox().ShowDialog();

            if (msbResult == DialogResult.OK)
            {
                var result = ofdDocument.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(ofdDocument.FileName))
                    {
                        _currentDocument = ofdDocument.FileName;
                    }

                    var fullTempPath = $"{_documentPath}{TemporaryFileName}";
                    var contract = new ContractReaderV2.ReaderV2(_currentDocument, fullTempPath);
                    var fileType = new Logic.FileExtensionHandler().GetDocumentType(_currentDocument);
                    switch (fileType)
                    {
                        case Logic.FileExtensionHandler.FileType.WordDoc:
                            _documentLines = contract.ParseWordDocument(_keywords);
                            break;
                        case Logic.FileExtensionHandler.FileType.Pdf:
                            _documentLines = contract.ParsePdfDocument(_keywords);
                            break;
                        default:
                            MessageBox.Show($@"Error => Error occured while deriving file type{Environment.NewLine}Either file was not found or the file type was unsupported", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _documentLines = null;
                            break;
                    }
                    if (_documentLines == null)
                    {
                        MessageBox.Show($@"Error => Error opening document{Environment.NewLine}Please make sure the document is not already open.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        dataGridView1.DataSource = _documentLines;
                        dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.Green;
                        dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.Yellow;
                        dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Black;

                        label2.Text = _currentDocument;
                    }
                }
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            if (_documentLines != null)
            {
                try
                {
                    new Logic.FileExportHandler().LinesToDoc(_documentLines,_outputDocumentPath);
                    MessageBox.Show($@"Export complete {Environment.NewLine} File saved to {_outputDocumentPath}");
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
            var frm = new frmSettings();
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
            this.Close();
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
    }
}
