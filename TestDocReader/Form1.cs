using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace TestDocReader
{
    public partial class Form1 : Form
    {
        private string _documentPath = System.Configuration.ConfigurationManager.AppSettings.Get("DocumentPath");
        private const string TEMPORARY_FILE_NAME = "Temp_Output.txt";
        private string _outputDocumentPath = $"{System.Configuration.ConfigurationManager.AppSettings.Get("OutputDocumentPath")}output.html";
        
        private string _currentDocument;
        
        private List<ContractReaderV2.Concrete.Contract> _documentLines;
        public Form1()
        {
            InitializeComponent();
            _documentLines = new List<ContractReaderV2.Concrete.Contract>();
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
            
            var fullTempPath = $"{_documentPath}{TEMPORARY_FILE_NAME}";
            var contract = new ContractReaderV2.Reader(_currentDocument, fullTempPath, ContractReaderV2.Concrete.Enum.GlobalEnum.DocumentType.doc);
            var fileType = new Logic.FileExtentionHandler().GetDocumentType(_currentDocument);
            if(fileType == Logic.FileExtentionHandler.FileType.WordDoc)
            {
                _documentLines = contract.ParseWordDocument();
            }
            else if(fileType == Logic.FileExtentionHandler.FileType.Pdf)
            {
                _documentLines = contract.ParsePdfDocument();
            }
            else
            {
                MessageBox.Show($"Error => Error occured while deriving file type {Environment.NewLine} Either file was not found or tye file type was unsupported");
            }


            dataGridView1.DataSource = _documentLines;
            dataGridView1.Columns[0].DefaultCellStyle.ForeColor = Color.Green;
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.Yellow;
            dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Black;

            label2.Text = _currentDocument;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        

        private void btnOutput_Click(object sender, EventArgs e)
        {
           
            if (_documentLines != null)
            {
                try
                {
                    new Logic.FileExportHandler().LinesToDoc(_documentLines,_outputDocumentPath);
                    MessageBox.Show($"Export complete {Environment.NewLine} File saved to {_outputDocumentPath}");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Please load a document first");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddToList_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(tbKeyword.Text))
            {
                lstKeyword.Items.Add(tbKeyword.Text);
                ModifyKeywordList();

            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            lstKeyword.Items.Remove(lstKeyword.SelectedItem);
            ModifyKeywordList();
        }

        private void ModifyKeywordList()
        {
            var keywordList = new List<string>();
            foreach (var item in lstKeyword.Items)
            {
                keywordList.Add(item.ToString());
                new Logic.KeywordConfigHandler().Add(keywordList);
            }
        }
    }
}
