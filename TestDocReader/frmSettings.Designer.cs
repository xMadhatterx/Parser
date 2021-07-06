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
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeywords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wordBindingSource)).BeginInit();
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
            this.lblSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSettings.ForeColor = System.Drawing.Color.White;
            this.lblSettings.Location = new System.Drawing.Point(11, 3);
            this.lblSettings.Name = "lblSettings";
            this.lblSettings.Size = new System.Drawing.Size(113, 31);
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
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(142)))), ((int)(((byte)(30)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, 354);
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
            this.dgvKeywords.Location = new System.Drawing.Point(12, 51);
            this.dgvKeywords.Name = "dgvKeywords";
            this.dgvKeywords.Size = new System.Drawing.Size(776, 221);
            this.dgvKeywords.TabIndex = 15;
            // 
            // btnAddRow
            // 
            this.btnAddRow.Location = new System.Drawing.Point(632, 288);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(75, 23);
            this.btnAddRow.TabIndex = 16;
            this.btnAddRow.Text = "Add";
            this.btnAddRow.UseVisualStyleBackColor = true;
            this.btnAddRow.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(713, 288);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // wordBindingSource
            // 
            this.wordBindingSource.DataSource = typeof(ContractReaderV2.Concrete.Word);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(46)))), ((int)(((byte)(59)))));
            this.ClientSize = new System.Drawing.Size(800, 396);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAddRow);
            this.Controls.Add(this.dgvKeywords);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSettings";
            this.Text = "frmSettings";
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKeywords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wordBindingSource)).EndInit();
            this.ResumeLayout(false);

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
    }
}