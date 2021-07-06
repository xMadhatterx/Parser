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

        private void ModifyKeywordList()
        {
            new Logic.KeywordConfigHandler().Add(_keywords);
        }

        private void LoadKeywords()
        {
            //lstKeyword.Items.Clear();
            //_keywords = new Logic.KeywordConfigHandler().Import().Keywords;
            _keywordsV2 = new Logic.KeywordConfigHandler().ImportV2().Keywords;
            dgvKeywords.DataSource = _keywordsV2;
            //foreach (var keyword in _keywords)
            //{
            //    lstKeyword.Items.Add(keyword);
            //}
        }
        private void LoadReplacements()
        {
            _replacements = new Logic.ReplacementWordConfigHandler().Import().ReplaceWords;
        }

        //private void btnAddToList_Click_1(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(tbKeyword.Text))
        //    {
        //        lstKeyword.Items.Add(tbKeyword.Text);
        //        _keywords.Add(tbKeyword.Text);
        //        tbKeyword.Text = string.Empty;
        //        ModifyKeywordList();
        //    }
        //}

        //private void btnRemove_Click_1(object sender, EventArgs e)
        //{
        //    _keywords.Remove(lstKeyword.SelectedItem.ToString());
        //    lstKeyword.Items.Remove(lstKeyword.SelectedItem);

        //    ModifyKeywordList();
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            _keywordsV2.Add(new Word() { Keyword = string.Empty, Replacement = string.Empty });
            dgvKeywords.DataSource = null;
            dgvKeywords.DataSource = _keywordsV2;
            //dgvKeywords.Rows.Add();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var r = new Root();
            r.Keywords = new List<Word>();
            r.Keywords.AddRange(_keywordsV2);
            new TestDocReader.Logic.KeywordConfigHandler().ExportV2(r);
        }
    }
}
