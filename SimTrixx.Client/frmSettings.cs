using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ContractReaderV2.Concrete;

namespace TestDocReader
{
    public partial class frmSettings : Form
    {
        private List<string> _keywords;
        private List<Word> _keywordsV2;
        private List<string> _replacements;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private RadioButton selectedRb;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]

        public static extern bool ReleaseCapture();
        public frmSettings()
        {
            InitializeComponent();
            _keywords = new List<string>();
            _replacements = new List<string>();
            LoadKeywords();
            LoadReplacements();
        }

        private void btnCloseFrm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void ModifyKeywordList()
        //{
        //    new Logic.KeywordConfigHandler().Add(_keywords);
        //}

        private void LoadKeywords()
        {
            _keywordsV2 = new Logic.KeywordConfigHandler().ImportV2().Keywords;
            BuildGrid();
        }
        private void LoadReplacements()
        {
            _replacements = new Logic.ReplacementWordConfigHandler().Import().ReplaceWords;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            _keywordsV2.Add(new Word() { Keyword = string.Empty, Replacement = string.Empty });
            BuildGrid();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var r = new Root();
            r.Keywords = new List<Word>();
            //r.Settings = new CustomSettings();
            //r.Keywords.AddRange(_keywordsV2);
            //if(selectedRb.Text.ToLower() == "word doc")
            //{
            //    r.Settings.FileExportType = "1";
            //}
            //else if(selectedRb.Text.ToLower()=="legacy word doc")
            //{
            //    r.Settings.FileExportType = "2";
            //}
            //else
            //{
            //    r.Settings.FileExportType = "3";
            //}
       
            new TestDocReader.Logic.KeywordConfigHandler().ExportV2(r);
            this.Close();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null)
            {
                MessageBox.Show("Sender is not a RadioButton");
                return;
            }

            // Ensure that the RadioButton.Checked property
            // changed to true.
            if (rb.Checked)
            {
                // Keep track of the selected RadioButton by saving a reference
                // to it.
                selectedRb = rb;
            }
        }


        private void BuildGrid()
        {
            dgvKeywords.DataSource = null;
            dgvKeywords.DataSource = _keywordsV2;
            var i = dgvKeywords.ColumnCount;
            if (i < 4)
            {
                var col = new DataGridViewButtonColumn();
                col.UseColumnTextForButtonValue = true;
                col.Text = "X";
                col.HeaderText = "Delete";
                col.FlatStyle = FlatStyle.Popup;
                
                //col.DefaultCellStyle.ForeColor = Color.Red;
                
                col.Name = "btnDeleteRow";
                dgvKeywords.Columns.Add(col);
            }
            dgvKeywords.Columns["btnDeleteRow"].DisplayIndex = 3;
        }

        private void dgvKeywords_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&e.RowIndex >= 0)
            {
                _keywordsV2.RemoveAt(e.RowIndex);
                BuildGrid();
            }
        }
    }
}
