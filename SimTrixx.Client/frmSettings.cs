using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SimTrixx.Reader.Concrete;

namespace SimTrixx.Client
{
    public partial class frmSettings : Form
    {
        private List<string> _keywords;
        private BindingList<Word> _keywordsV2;
        private List<string> _replacements;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private RadioButton selectedRb;
        private Form1 main;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]

        public static extern bool ReleaseCapture();
        public frmSettings(Form1 mainForm)
        {
            main = mainForm;
            InitializeComponent();
            _keywords = new List<string>();
            _replacements = new List<string>();
            LoadKeywords();
            LoadReplacements();
            LoadLicense();
        }

        private void btnCloseFrm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadLicense()
        {
            txtLicense.Text = Properties.Settings.Default["License"].ToString();
        }
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
            _keywordsV2.Add(new Word() { Keyword = "Change Me", Replacement = "Change Me" });
            BuildGrid();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var r = new Root();
            r.Keywords = new BindingList<Word>();
            //r.Settings = new CustomSettings();
            foreach (var item in _keywordsV2)
            {
                r.Keywords.Add(item);
            }
            Properties.Settings.Default["License"] = txtLicense.Text;
            Properties.Settings.Default.Save();
            new Logic.KeywordConfigHandler().ExportV2(r);
            main.CheckLicense();
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
            dgvKeywords.ClearSelection();
            dgvKeywords.DataSource = null;
            dgvKeywords.DataSource = _keywordsV2;
            dgvKeywords.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            var i = dgvKeywords.ColumnCount;
            if (i < 4)
            {
                //var col = new DataGridViewButtonColumn();
                //col.UseColumnTextForButtonValue = true;
                //col.Text = "     ";

                //col.HeaderText = "Delete";
                //col.FlatStyle = FlatStyle.Flat;
                //col.Name = "btnDeleteRow";
                var col = new DataGridViewImageColumn();
                col.Image = Properties.Resources.deleteIcon;
                col.Name = "btnDeleteRow";
                col.HeaderText = "Delete";
                col.DefaultCellStyle.SelectionBackColor = Color.White;
                dgvKeywords.Columns.Add(col);
            }

            dgvKeywords.Columns["btnDeleteRow"].DisplayIndex = 3;
        }

        private void dgvKeywords_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (grid.Columns[e.ColumnIndex] is DataGridViewImageColumn &&e.RowIndex >= 0)
            {
                _keywordsV2.RemoveAt(e.RowIndex);
                BuildGrid();
            }
        }

        private void dgvKeywords_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //if (e.RowIndex < 0)
            //    return;

            ////I supposed your button column is at index 0
           
            //if (e.ColumnIndex == 3)
            //{
            //    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            //    var w = Properties.Resources.deleteIcon.Width;
            //    var h = Properties.Resources.deleteIcon.Height;
            //    var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
            //    var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

            //    e.Graphics.DrawImage(Properties.Resources.deleteIcon, new Rectangle(x, y, w, h));
            //    e.Handled = true;
            //}
        }
    }
}
