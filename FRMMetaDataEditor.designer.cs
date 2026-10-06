namespace CamulosSharePointUpload
{
    partial class FRMMetaDataEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRMMetaDataEditor));
            this.lstMetadataEditor = new System.Windows.Forms.ListBox();
            this.TXTpath = new System.Windows.Forms.TextBox();
            this.TXTupdate = new System.Windows.Forms.TextBox();
            this.TXTvalue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.BTNupdate = new System.Windows.Forms.Button();
            this.BTNadd = new System.Windows.Forms.Button();
            this.BTNdelete = new System.Windows.Forms.Button();
            this.BTNsaveFile = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lstMetadataEditor
            // 
            this.lstMetadataEditor.FormattingEnabled = true;
            this.lstMetadataEditor.ItemHeight = 17;
            this.lstMetadataEditor.Location = new System.Drawing.Point(12, 34);
            this.lstMetadataEditor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstMetadataEditor.Name = "lstMetadataEditor";
            this.lstMetadataEditor.Size = new System.Drawing.Size(326, 361);
            this.lstMetadataEditor.TabIndex = 0;
            this.lstMetadataEditor.SelectedIndexChanged += new System.EventHandler(this.lstMetadataEditor_SelectedIndexChanged);
            // 
            // TXTpath
            // 
            this.TXTpath.Location = new System.Drawing.Point(459, 171);
            this.TXTpath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TXTpath.Name = "TXTpath";
            this.TXTpath.Size = new System.Drawing.Size(213, 23);
            this.TXTpath.TabIndex = 1;
            // 
            // TXTupdate
            // 
            this.TXTupdate.Location = new System.Drawing.Point(459, 213);
            this.TXTupdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TXTupdate.Name = "TXTupdate";
            this.TXTupdate.Size = new System.Drawing.Size(213, 23);
            this.TXTupdate.TabIndex = 2;
            // 
            // TXTvalue
            // 
            this.TXTvalue.Location = new System.Drawing.Point(459, 253);
            this.TXTvalue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TXTvalue.Name = "TXTvalue";
            this.TXTvalue.Size = new System.Drawing.Size(213, 23);
            this.TXTvalue.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(349, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Path Query";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(349, 256);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Value";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(692, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.addFromFileToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // addFromFileToolStripMenuItem
            // 
            this.addFromFileToolStripMenuItem.Name = "addFromFileToolStripMenuItem";
            this.addFromFileToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.addFromFileToolStripMenuItem.Text = "Add From File";
            this.addFromFileToolStripMenuItem.Click += new System.EventHandler(this.addFromFileToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(349, 216);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Field to Update";
            // 
            // BTNupdate
            // 
            this.BTNupdate.Location = new System.Drawing.Point(359, 321);
            this.BTNupdate.Name = "BTNupdate";
            this.BTNupdate.Size = new System.Drawing.Size(148, 29);
            this.BTNupdate.TabIndex = 8;
            this.BTNupdate.Text = "Update";
            this.BTNupdate.UseVisualStyleBackColor = true;
            this.BTNupdate.Click += new System.EventHandler(this.BTNupdate_Click);
            // 
            // BTNadd
            // 
            this.BTNadd.Location = new System.Drawing.Point(522, 321);
            this.BTNadd.Name = "BTNadd";
            this.BTNadd.Size = new System.Drawing.Size(148, 29);
            this.BTNadd.TabIndex = 9;
            this.BTNadd.Text = "Add";
            this.BTNadd.UseVisualStyleBackColor = true;
            this.BTNadd.Click += new System.EventHandler(this.BTNadd_Click);
            // 
            // BTNdelete
            // 
            this.BTNdelete.Location = new System.Drawing.Point(357, 356);
            this.BTNdelete.Name = "BTNdelete";
            this.BTNdelete.Size = new System.Drawing.Size(148, 29);
            this.BTNdelete.TabIndex = 10;
            this.BTNdelete.Text = "Delete";
            this.BTNdelete.UseVisualStyleBackColor = true;
            this.BTNdelete.Click += new System.EventHandler(this.BTNdelete_Click);
            // 
            // BTNsaveFile
            // 
            this.BTNsaveFile.Location = new System.Drawing.Point(522, 356);
            this.BTNsaveFile.Name = "BTNsaveFile";
            this.BTNsaveFile.Size = new System.Drawing.Size(150, 29);
            this.BTNsaveFile.TabIndex = 11;
            this.BTNsaveFile.Text = "Save File";
            this.BTNsaveFile.UseVisualStyleBackColor = true;
            this.BTNsaveFile.Click += new System.EventHandler(this.BTNsaveFile_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(471, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(209, 73);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(352, 127);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(318, 25);
            this.comboBox1.TabIndex = 13;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // FRMMetaDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(692, 407);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.BTNsaveFile);
            this.Controls.Add(this.BTNdelete);
            this.Controls.Add(this.BTNadd);
            this.Controls.Add(this.BTNupdate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TXTvalue);
            this.Controls.Add(this.TXTupdate);
            this.Controls.Add(this.TXTpath);
            this.Controls.Add(this.lstMetadataEditor);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FRMMetaDataEditor";
            this.Text = "FRMMetaDataEditor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstMetadataEditor;
        private System.Windows.Forms.TextBox TXTpath;
        private System.Windows.Forms.TextBox TXTupdate;
        private System.Windows.Forms.TextBox TXTvalue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BTNupdate;
        private System.Windows.Forms.Button BTNadd;
        private System.Windows.Forms.Button BTNdelete;
        private System.Windows.Forms.Button BTNsaveFile;
        private System.Windows.Forms.ToolStripMenuItem addFromFileToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}