using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace TestDocReader
{
    public partial class Form1 : Form
    {
        private readonly string _documentPath = System.Configuration.ConfigurationManager.AppSettings.Get("DocumentPath");
        private const string TemporaryFileName = "Temp_Output.txt";
        private readonly string _outputDocumentPath = $"{System.Configuration.ConfigurationManager.AppSettings.Get("OutputDocumentPath")}output.html";
        private string _currentDocument;
        private List<ContractReaderV2.Concrete.Contract> _documentLines;
        private List<string> _keywords;

        public Form1()
        {
            InitializeComponent();
            _documentLines = new List<ContractReaderV2.Concrete.Contract>();
            _keywords = new List<string>();
            LoadKeywords();
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            var result = ofdDocument.ShowDialog();
            if(result == DialogResult.OK)
            {
                if(!string.IsNullOrWhiteSpace(ofdDocument.FileName))
                {
                    _currentDocument = ofdDocument.FileName;
                }
            }
            
            var fullTempPath = $"{_documentPath}{TemporaryFileName}";
            var contract = new ContractReaderV2.Reader(_currentDocument, fullTempPath);//, ContractReaderV2.Concrete.Enum.GlobalEnum.DocumentType.doc);
            var fileType = new Logic.FileExtensionHandler().GetDocumentType(_currentDocument);
            if(fileType == Logic.FileExtensionHandler.FileType.WordDoc)
            {
                _documentLines = contract.ParseWordDocument(_keywords);
            }
            else if(fileType == Logic.FileExtensionHandler.FileType.Pdf)
            {
                _documentLines = contract.ParsePdfDocument(_keywords);
            }
            else
            {
                MessageBox.Show($@"Error => Error occured while deriving file type {Environment.NewLine} Either file was not found or tye file type was unsupported");
            }


            dataGridView1.DataSource = _documentLines;
            dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.Green;
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.Yellow;
            dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Black;

            label2.Text = _currentDocument;

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
        private void btnAddToList_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(tbKeyword.Text))
            {
                lstKeyword.Items.Add(tbKeyword.Text);
                _keywords.Add(tbKeyword.Text);
                tbKeyword.Text = string.Empty;
                ModifyKeywordList();
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            _keywords.Remove(lstKeyword.SelectedItem.ToString());
            lstKeyword.Items.Remove(lstKeyword.SelectedItem);
            
            ModifyKeywordList();
        }
        private void ModifyKeywordList()
        {
            new Logic.KeywordConfigHandler().Add(_keywords);

        }
        private void LoadKeywords()
        {
            lstKeyword.Items.Clear();
             _keywords = new Logic.KeywordConfigHandler().Import().Keywords;
            foreach(var keyword in _keywords)
            {
                lstKeyword.Items.Add(keyword);
            }
        }
    }
}
