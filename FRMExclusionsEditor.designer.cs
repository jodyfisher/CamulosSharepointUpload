namespace CamulosSharePointUpload
{
    partial class FRMExclusionsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRMExclusionsEditor));
            this.lstExclusions = new System.Windows.Forms.ListBox();
            this.BTNupdate = new System.Windows.Forms.Button();
            this.BTNdelete = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MNUsave = new System.Windows.Forms.ToolStripMenuItem();
            this.MNUsaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.TXTlineItem = new System.Windows.Forms.TextBox();
            this.BTNsaveFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TXTegFileName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BTNtest = new System.Windows.Forms.Button();
            this.BTNadd = new System.Windows.Forms.Button();
            this.addFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lstExclusions
            // 
            this.lstExclusions.FormattingEnabled = true;
            this.lstExclusions.ItemHeight = 17;
            this.lstExclusions.Location = new System.Drawing.Point(16, 44);
            this.lstExclusions.Margin = new System.Windows.Forms.Padding(4);
            this.lstExclusions.Name = "lstExclusions";
            this.lstExclusions.Size = new System.Drawing.Size(367, 412);
            this.lstExclusions.TabIndex = 0;
            this.lstExclusions.SelectedIndexChanged += new System.EventHandler(this.lstExclusions_SelectedIndexChanged);
            // 
            // BTNupdate
            // 
            this.BTNupdate.Location = new System.Drawing.Point(410, 379);
            this.BTNupdate.Margin = new System.Windows.Forms.Padding(4);
            this.BTNupdate.Name = "BTNupdate";
            this.BTNupdate.Size = new System.Drawing.Size(151, 33);
            this.BTNupdate.TabIndex = 1;
            this.BTNupdate.Text = "Update";
            this.BTNupdate.UseVisualStyleBackColor = true;
            this.BTNupdate.Click += new System.EventHandler(this.BTNupdate_Click);
            // 
            // BTNdelete
            // 
            this.BTNdelete.Location = new System.Drawing.Point(410, 420);
            this.BTNdelete.Margin = new System.Windows.Forms.Padding(4);
            this.BTNdelete.Name = "BTNdelete";
            this.BTNdelete.Size = new System.Drawing.Size(151, 33);
            this.BTNdelete.TabIndex = 2;
            this.BTNdelete.Text = "Delete";
            this.BTNdelete.UseVisualStyleBackColor = true;
            this.BTNdelete.Click += new System.EventHandler(this.BTNdelete_Click);
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
            this.menuStrip1.Size = new System.Drawing.Size(759, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.addFromFileToolStripMenuItem,
            this.MNUsave,
            this.MNUsaveAs});
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
            // MNUsave
            // 
            this.MNUsave.Name = "MNUsave";
            this.MNUsave.Size = new System.Drawing.Size(180, 22);
            this.MNUsave.Text = "Save";
            this.MNUsave.Click += new System.EventHandler(this.MNUsave_Click);
            // 
            // MNUsaveAs
            // 
            this.MNUsaveAs.Name = "MNUsaveAs";
            this.MNUsaveAs.Size = new System.Drawing.Size(180, 22);
            this.MNUsaveAs.Text = "Save As";
            this.MNUsaveAs.Click += new System.EventHandler(this.MNUsaveAs_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "File Name",
            "Regular Expression"});
            this.comboBox1.Location = new System.Drawing.Point(409, 125);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(331, 25);
            this.comboBox1.TabIndex = 4;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // TXTlineItem
            // 
            this.TXTlineItem.Location = new System.Drawing.Point(409, 156);
            this.TXTlineItem.Name = "TXTlineItem";
            this.TXTlineItem.Size = new System.Drawing.Size(331, 23);
            this.TXTlineItem.TabIndex = 5;
            this.TXTlineItem.TextChanged += new System.EventHandler(this.TXTlineItem_TextChanged);
            // 
            // BTNsaveFile
            // 
            this.BTNsaveFile.Location = new System.Drawing.Point(590, 420);
            this.BTNsaveFile.Name = "BTNsaveFile";
            this.BTNsaveFile.Size = new System.Drawing.Size(151, 34);
            this.BTNsaveFile.TabIndex = 6;
            this.BTNsaveFile.Text = "Save File";
            this.BTNsaveFile.UseVisualStyleBackColor = true;
            this.BTNsaveFile.Click += new System.EventHandler(this.BTNsaveFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(406, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Test the Regular Expression";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(406, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Example File Name";
            this.label2.Visible = false;
            // 
            // TXTegFileName
            // 
            this.TXTegFileName.Location = new System.Drawing.Point(550, 233);
            this.TXTegFileName.Name = "TXTegFileName";
            this.TXTegFileName.Size = new System.Drawing.Size(197, 23);
            this.TXTegFileName.TabIndex = 9;
            this.TXTegFileName.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(548, 273);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "label3";
            this.label3.Visible = false;
            // 
            // BTNtest
            // 
            this.BTNtest.Location = new System.Drawing.Point(535, 303);
            this.BTNtest.Name = "BTNtest";
            this.BTNtest.Size = new System.Drawing.Size(75, 23);
            this.BTNtest.TabIndex = 11;
            this.BTNtest.Text = "Test";
            this.BTNtest.UseVisualStyleBackColor = true;
            this.BTNtest.Visible = false;
            this.BTNtest.Click += new System.EventHandler(this.BTNtest_Click);
            // 
            // BTNadd
            // 
            this.BTNadd.Location = new System.Drawing.Point(590, 379);
            this.BTNadd.Name = "BTNadd";
            this.BTNadd.Size = new System.Drawing.Size(151, 33);
            this.BTNadd.TabIndex = 12;
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
            this.pictureBox1.Location = new System.Drawing.Point(526, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(221, 78);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // FRMExclusionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(759, 466);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.BTNadd);
            this.Controls.Add(this.BTNtest);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TXTegFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BTNsaveFile);
            this.Controls.Add(this.TXTlineItem);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.BTNdelete);
            this.Controls.Add(this.BTNupdate);
            this.Controls.Add(this.lstExclusions);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FRMExclusionsEditor";
            this.Text = "Form Exclusions Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstExclusions;
        private System.Windows.Forms.Button BTNupdate;
        private System.Windows.Forms.Button BTNdelete;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MNUsave;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox TXTlineItem;
        private System.Windows.Forms.Button BTNsaveFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TXTegFileName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BTNtest;
        private System.Windows.Forms.ToolStripMenuItem MNUsaveAs;
        private System.Windows.Forms.Button BTNadd;
        private System.Windows.Forms.ToolStripMenuItem addFromFileToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}