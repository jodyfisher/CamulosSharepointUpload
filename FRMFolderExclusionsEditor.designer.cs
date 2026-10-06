namespace CamulosSharePointUpload
{
    partial class FRMFolderExclusionsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRMFolderExclusionsEditor));
            this.lstFolderExclusions = new System.Windows.Forms.ListBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TXTlineItem = new System.Windows.Forms.TextBox();
            this.BTNupdate = new System.Windows.Forms.Button();
            this.BTNdelete = new System.Windows.Forms.Button();
            this.TXTegRegexFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BTNtest = new System.Windows.Forms.Button();
            this.BTNsaveFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BTNbrowse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.BTNadd = new System.Windows.Forms.Button();
            this.addFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lstFolderExclusions
            // 
            this.lstFolderExclusions.FormattingEnabled = true;
            this.lstFolderExclusions.ItemHeight = 17;
            this.lstFolderExclusions.Location = new System.Drawing.Point(13, 35);
            this.lstFolderExclusions.Margin = new System.Windows.Forms.Padding(4);
            this.lstFolderExclusions.Name = "lstFolderExclusions";
            this.lstFolderExclusions.Size = new System.Drawing.Size(392, 395);
            this.lstFolderExclusions.TabIndex = 0;
            this.lstFolderExclusions.SelectedIndexChanged += new System.EventHandler(this.lstFolderExclusions_SelectedIndexChanged_1);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "File Name",
            "Regular Expression"});
            this.comboBox1.Location = new System.Drawing.Point(432, 118);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(321, 25);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged_1);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(789, 24);
            this.menuStrip1.TabIndex = 2;
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
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // TXTlineItem
            // 
            this.TXTlineItem.Location = new System.Drawing.Point(432, 159);
            this.TXTlineItem.Name = "TXTlineItem";
            this.TXTlineItem.Size = new System.Drawing.Size(230, 23);
            this.TXTlineItem.TabIndex = 3;
            // 
            // BTNupdate
            // 
            this.BTNupdate.Location = new System.Drawing.Point(432, 346);
            this.BTNupdate.Name = "BTNupdate";
            this.BTNupdate.Size = new System.Drawing.Size(150, 28);
            this.BTNupdate.TabIndex = 4;
            this.BTNupdate.Text = "Update";
            this.BTNupdate.UseVisualStyleBackColor = true;
            this.BTNupdate.Click += new System.EventHandler(this.BTNupdate_Click);
            // 
            // BTNdelete
            // 
            this.BTNdelete.Location = new System.Drawing.Point(432, 380);
            this.BTNdelete.Name = "BTNdelete";
            this.BTNdelete.Size = new System.Drawing.Size(150, 28);
            this.BTNdelete.TabIndex = 5;
            this.BTNdelete.Text = "Delete";
            this.BTNdelete.UseVisualStyleBackColor = true;
            this.BTNdelete.Click += new System.EventHandler(this.BTNdelete_Click);
            // 
            // TXTegRegexFile
            // 
            this.TXTegRegexFile.Location = new System.Drawing.Point(594, 235);
            this.TXTegRegexFile.Name = "TXTegRegexFile";
            this.TXTegRegexFile.Size = new System.Drawing.Size(165, 23);
            this.TXTegRegexFile.TabIndex = 6;
            this.TXTegRegexFile.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(432, 238);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Example Folder Name";
            this.label1.Visible = false;
            // 
            // BTNtest
            // 
            this.BTNtest.Location = new System.Drawing.Point(561, 296);
            this.BTNtest.Name = "BTNtest";
            this.BTNtest.Size = new System.Drawing.Size(75, 23);
            this.BTNtest.TabIndex = 8;
            this.BTNtest.Text = "Test";
            this.BTNtest.UseVisualStyleBackColor = true;
            this.BTNtest.Visible = false;
            this.BTNtest.Click += new System.EventHandler(this.BTNtest_Click);
            // 
            // BTNsaveFile
            // 
            this.BTNsaveFile.Location = new System.Drawing.Point(609, 380);
            this.BTNsaveFile.Name = "BTNsaveFile";
            this.BTNsaveFile.Size = new System.Drawing.Size(150, 28);
            this.BTNsaveFile.TabIndex = 9;
            this.BTNsaveFile.Text = "Save File";
            this.BTNsaveFile.UseVisualStyleBackColor = true;
            this.BTNsaveFile.Click += new System.EventHandler(this.BTNsaveFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(432, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Test the Regular Expression";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(574, 267);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "label3";
            this.label3.Visible = false;
            // 
            // BTNbrowse
            // 
            this.BTNbrowse.Location = new System.Drawing.Point(684, 159);
            this.BTNbrowse.Name = "BTNbrowse";
            this.BTNbrowse.Size = new System.Drawing.Size(75, 23);
            this.BTNbrowse.TabIndex = 12;
            this.BTNbrowse.Text = "Browse...";
            this.BTNbrowse.UseVisualStyleBackColor = true;
            this.BTNbrowse.Click += new System.EventHandler(this.BTNbrowse_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // BTNadd
            // 
            this.BTNadd.Location = new System.Drawing.Point(609, 346);
            this.BTNadd.Name = "BTNadd";
            this.BTNadd.Size = new System.Drawing.Size(150, 28);
            this.BTNadd.TabIndex = 13;
            this.BTNadd.Text = "Add";
            this.BTNadd.UseVisualStyleBackColor = true;
            this.BTNadd.Click += new System.EventHandler(this.BTNadd_Click);
            // 
            // addFromFileToolStripMenuItem
            // 
            this.addFromFileToolStripMenuItem.Name = "addFromFileToolStripMenuItem";
            this.addFromFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addFromFileToolStripMenuItem.Text = "Add From File";
            this.addFromFileToolStripMenuItem.Click += new System.EventHandler(this.addFromFileToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(577, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(207, 76);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // FRMFolderExclusionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(789, 440);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.BTNadd);
            this.Controls.Add(this.BTNbrowse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BTNsaveFile);
            this.Controls.Add(this.BTNtest);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TXTegRegexFile);
            this.Controls.Add(this.BTNdelete);
            this.Controls.Add(this.BTNupdate);
            this.Controls.Add(this.TXTlineItem);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.lstFolderExclusions);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FRMFolderExclusionsEditor";
            this.Text = "Folder Exclusions Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstFolderExclusions;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.TextBox TXTlineItem;
        private System.Windows.Forms.Button BTNupdate;
        private System.Windows.Forms.Button BTNdelete;
        private System.Windows.Forms.TextBox TXTegRegexFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BTNtest;
        private System.Windows.Forms.Button BTNsaveFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BTNbrowse;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button BTNadd;
        private System.Windows.Forms.ToolStripMenuItem addFromFileToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}