namespace TestDocReader
{
    partial class frmSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblSettings = new System.Windows.Forms.Label();
            this.btnCloseFrm = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvKeywords = new System.Windows.Forms.DataGridView();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.wordBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbWord = new System.Windows.Forms.RadioButton();
            this.rbHtml = new System.Windows.Forms.RadioButton();
            this.rbExcel = new System.Windows.Forms.RadioButton();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeywords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wordBindingSource)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(142)))), ((int)(((byte)(30)))));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.lblSettings);
            this.panel4.Controls.Add(this.btnCloseFrm);
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(800, 45);
            this.panel4.TabIndex = 7;
            // 
            // lblSettings
            // 
            this.lblSettings.AutoSize = true;
            this.lblSettings.BackColor = System.Drawing.Color.Transparent;
            this.lblSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSettings.ForeColor = System.Drawing.Color.White;
            this.lblSettings.Location = new System.Drawing.Point(11, 3);
            this.lblSettings.Name = "lblSettings";
            this.lblSettings.Size = new System.Drawing.Size(121, 31);
            this.lblSettings.TabIndex = 9;
            this.lblSettings.Text = "Settings";
            // 
            // btnCloseFrm
            // 
            this.btnCloseFrm.BackColor = System.Drawing.Color.Transparent;
            this.btnCloseFrm.FlatAppearance.BorderSize = 0;
            this.btnCloseFrm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCloseFrm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCloseFrm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseFrm.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCloseFrm.ForeColor = System.Drawing.Color.White;
            this.btnCloseFrm.Location = new System.Drawing.Point(750, 3);
            this.btnCloseFrm.Name = "btnCloseFrm";
            this.btnCloseFrm.Size = new System.Drawing.Size(45, 28);
            this.btnCloseFrm.TabIndex = 8;
            this.btnCloseFrm.Text = "X";
            this.btnCloseFrm.UseVisualStyleBackColor = false;
            this.btnCloseFrm.Click += new System.EventHandler(this.btnCloseFrm_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(142)))), ((int)(((byte)(30)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Location = new System.Drawing.Point(0, 504);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 45);
            this.panel1.TabIndex = 10;
            // 
            // dgvKeywords
            // 
            this.dgvKeywords.AllowUserToResizeColumns = false;
            this.dgvKeywords.AllowUserToResizeRows = false;
            this.dgvKeywords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvKeywords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKeywords.Location = new System.Drawing.Point(12, 94);
            this.dgvKeywords.Name = "dgvKeywords";
            this.dgvKeywords.Size = new System.Drawing.Size(776, 221);
            this.dgvKeywords.TabIndex = 15;
            this.dgvKeywords.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvKeywords_CellContentClick);
            // 
            // btnAddRow
            // 
            this.btnAddRow.Location = new System.Drawing.Point(713, 321);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(75, 23);
            this.btnAddRow.TabIndex = 16;
            this.btnAddRow.Text = "New";
            this.btnAddRow.UseVisualStyleBackColor = true;
            this.btnAddRow.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(712, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // wordBindingSource
            // 
            this.wordBindingSource.DataSource = typeof(SimTrixx.Reader.Concrete.Word);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(18, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 25);
            this.label1.TabIndex = 18;
            this.label1.Text = "KEYWORDS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(18, 363);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 25);
            this.label2.TabIndex = 19;
            this.label2.Text = "File Export";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbExcel);
            this.panel2.Controls.Add(this.rbHtml);
            this.panel2.Controls.Add(this.rbWord);
            this.panel2.Location = new System.Drawing.Point(23, 392);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(765, 100);
            this.panel2.TabIndex = 20;
            // 
            // rbWord
            // 
            this.rbWord.AutoSize = true;
            this.rbWord.Checked = true;
            this.rbWord.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rbWord.Location = new System.Drawing.Point(44, 39);
            this.rbWord.Name = "rbWord";
            this.rbWord.Size = new System.Drawing.Size(74, 17);
            this.rbWord.TabIndex = 0;
            this.rbWord.TabStop = true;
            this.rbWord.Text = "Word Doc";
            this.rbWord.UseVisualStyleBackColor = true;
            this.rbWord.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // rbHtml
            // 
            this.rbHtml.AutoSize = true;
            this.rbHtml.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rbHtml.Location = new System.Drawing.Point(136, 39);
            this.rbHtml.Name = "rbHtml";
            this.rbHtml.Size = new System.Drawing.Size(112, 17);
            this.rbHtml.TabIndex = 1;
            this.rbHtml.Text = "Legacy Word Doc";
            this.rbHtml.UseVisualStyleBackColor = true;
            this.rbHtml.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // rbExcel
            // 
            this.rbExcel.AutoSize = true;
            this.rbExcel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rbExcel.Location = new System.Drawing.Point(263, 39);
            this.rbExcel.Name = "rbExcel";
            this.rbExcel.Size = new System.Drawing.Size(51, 17);
            this.rbExcel.TabIndex = 2;
            this.rbExcel.Text = "Excel";
            this.rbExcel.UseVisualStyleBackColor = true;
            this.rbExcel.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(46)))), ((int)(((byte)(59)))));
            this.ClientSize = new System.Drawing.Size(800, 546);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAddRow);
            this.Controls.Add(this.dgvKeywords);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSettings";
            this.Text = "frmSettings";
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeywords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wordBindingSource)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnCloseFrm;
        private System.Windows.Forms.Label lblSettings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvKeywords;
        private System.Windows.Forms.BindingSource wordBindingSource;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbExcel;
        private System.Windows.Forms.RadioButton rbHtml;
        private System.Windows.Forms.RadioButton rbWord;
    }
}