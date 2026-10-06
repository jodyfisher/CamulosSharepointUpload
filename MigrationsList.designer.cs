namespace CamulosSharePointUpload
{
    partial class MigrationsList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MigrationsList));
            this.lstMigrations = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.BTNAdd = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMetadataMigrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileExclusionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderExclusionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metaDataQueriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logAnalyserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.lstMetadataMigrations = new System.Windows.Forms.ListBox();
            this.BTNmetadataEdit = new System.Windows.Forms.Button();
            this.BTNmetadataAdd = new System.Windows.Forms.Button();
            this.BTNmetadataCopy = new System.Windows.Forms.Button();
            this.BTNmetadataDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRunMigrations = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstMigrations
            // 
            this.lstMigrations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstMigrations.FormattingEnabled = true;
            this.lstMigrations.ItemHeight = 17;
            this.lstMigrations.Location = new System.Drawing.Point(31, 39);
            this.lstMigrations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstMigrations.Name = "lstMigrations";
            this.lstMigrations.Size = new System.Drawing.Size(263, 208);
            this.lstMigrations.TabIndex = 0;
            this.lstMigrations.SelectedIndexChanged += new System.EventHandler(this.lstMigrations_SelectedIndexChanged);
            this.lstMigrations.DoubleClick += new System.EventHandler(this.openMigration);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(320, 65);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 31);
            this.button1.TabIndex = 1;
            this.button1.Text = "Edit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(474, 111);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(136, 31);
            this.button2.TabIndex = 2;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(320, 111);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(136, 31);
            this.button3.TabIndex = 3;
            this.button3.Text = "Copy";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // BTNAdd
            // 
            this.BTNAdd.Location = new System.Drawing.Point(474, 65);
            this.BTNAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BTNAdd.Name = "BTNAdd";
            this.BTNAdd.Size = new System.Drawing.Size(136, 31);
            this.BTNAdd.TabIndex = 4;
            this.BTNAdd.Text = "Add";
            this.BTNAdd.UseVisualStyleBackColor = true;
            this.BTNAdd.Click += new System.EventHandler(this.BTNAdd_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(648, 130);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(298, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "CAMULOS MIGRATOR TOOL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(649, 156);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(284, 19);
            this.label2.TabIndex = 6;
            this.label2.Text = "Welcome to the Camulos Migrator Tool. ";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(720, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(226, 89);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(949, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.openMetadataMigrationToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // openMetadataMigrationToolStripMenuItem
            // 
            this.openMetadataMigrationToolStripMenuItem.Name = "openMetadataMigrationToolStripMenuItem";
            this.openMetadataMigrationToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.openMetadataMigrationToolStripMenuItem.Text = "Open Metadata Migration";
            this.openMetadataMigrationToolStripMenuItem.Click += new System.EventHandler(this.openMetadataMigrationToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileExclusionsToolStripMenuItem,
            this.folderExclusionsToolStripMenuItem,
            this.metaDataQueriesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // fileExclusionsToolStripMenuItem
            // 
            this.fileExclusionsToolStripMenuItem.Name = "fileExclusionsToolStripMenuItem";
            this.fileExclusionsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.fileExclusionsToolStripMenuItem.Text = "File Exclusions";
            this.fileExclusionsToolStripMenuItem.Click += new System.EventHandler(this.fileExclusionsToolStripMenuItem_Click);
            // 
            // folderExclusionsToolStripMenuItem
            // 
            this.folderExclusionsToolStripMenuItem.Name = "folderExclusionsToolStripMenuItem";
            this.folderExclusionsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.folderExclusionsToolStripMenuItem.Text = "Folder Exclusions";
            this.folderExclusionsToolStripMenuItem.Click += new System.EventHandler(this.folderExclusionsToolStripMenuItem_Click);
            // 
            // metaDataQueriesToolStripMenuItem
            // 
            this.metaDataQueriesToolStripMenuItem.Name = "metaDataQueriesToolStripMenuItem";
            this.metaDataQueriesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.metaDataQueriesToolStripMenuItem.Text = "Meta Data Queries";
            this.metaDataQueriesToolStripMenuItem.Click += new System.EventHandler(this.metaDataQueriesToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logAnalyserToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // logAnalyserToolStripMenuItem
            // 
            this.logAnalyserToolStripMenuItem.Name = "logAnalyserToolStripMenuItem";
            this.logAnalyserToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.logAnalyserToolStripMenuItem.Text = "Log Analyser";
            this.logAnalyserToolStripMenuItem.Click += new System.EventHandler(this.logAnalyserToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lstMetadataMigrations
            // 
            this.lstMetadataMigrations.FormattingEnabled = true;
            this.lstMetadataMigrations.ItemHeight = 17;
            this.lstMetadataMigrations.Location = new System.Drawing.Point(31, 266);
            this.lstMetadataMigrations.Name = "lstMetadataMigrations";
            this.lstMetadataMigrations.Size = new System.Drawing.Size(263, 208);
            this.lstMetadataMigrations.TabIndex = 9;
            this.lstMetadataMigrations.SelectedIndexChanged += new System.EventHandler(this.lstMetadataMigrations_SelectedIndexChanged);
            this.lstMetadataMigrations.DoubleClick += new System.EventHandler(this.lstMetadataMigrations_DoubleClick);
            // 
            // BTNmetadataEdit
            // 
            this.BTNmetadataEdit.Location = new System.Drawing.Point(320, 295);
            this.BTNmetadataEdit.Name = "BTNmetadataEdit";
            this.BTNmetadataEdit.Size = new System.Drawing.Size(136, 31);
            this.BTNmetadataEdit.TabIndex = 10;
            this.BTNmetadataEdit.Text = "Edit";
            this.BTNmetadataEdit.UseVisualStyleBackColor = true;
            this.BTNmetadataEdit.Click += new System.EventHandler(this.BTNmetadataEdit_Click);
            // 
            // BTNmetadataAdd
            // 
            this.BTNmetadataAdd.Location = new System.Drawing.Point(474, 295);
            this.BTNmetadataAdd.Name = "BTNmetadataAdd";
            this.BTNmetadataAdd.Size = new System.Drawing.Size(136, 31);
            this.BTNmetadataAdd.TabIndex = 11;
            this.BTNmetadataAdd.Text = "Add";
            this.BTNmetadataAdd.UseVisualStyleBackColor = true;
            this.BTNmetadataAdd.Click += new System.EventHandler(this.BTNmetadataAdd_Click);
            // 
            // BTNmetadataCopy
            // 
            this.BTNmetadataCopy.Location = new System.Drawing.Point(320, 344);
            this.BTNmetadataCopy.Name = "BTNmetadataCopy";
            this.BTNmetadataCopy.Size = new System.Drawing.Size(136, 31);
            this.BTNmetadataCopy.TabIndex = 12;
            this.BTNmetadataCopy.Text = "Copy";
            this.BTNmetadataCopy.UseVisualStyleBackColor = true;
            // 
            // BTNmetadataDelete
            // 
            this.BTNmetadataDelete.Location = new System.Drawing.Point(474, 344);
            this.BTNmetadataDelete.Name = "BTNmetadataDelete";
            this.BTNmetadataDelete.Size = new System.Drawing.Size(136, 31);
            this.BTNmetadataDelete.TabIndex = 13;
            this.BTNmetadataDelete.Text = "Delete";
            this.BTNmetadataDelete.UseVisualStyleBackColor = true;
            this.BTNmetadataDelete.Click += new System.EventHandler(this.BTNmetadataDelete_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(758, 216);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(136, 31);
            this.btnRefresh.TabIndex = 14;
            this.btnRefresh.Text = "Copy";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(316, 40);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 22);
            this.label3.TabIndex = 15;
            this.label3.Text = "Migrations";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(316, 270);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 22);
            this.label4.TabIndex = 16;
            this.label4.Text = "Metadata";
            // 
            // btnRunMigrations
            // 
            this.btnRunMigrations.Location = new System.Drawing.Point(320, 156);
            this.btnRunMigrations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRunMigrations.Name = "btnRunMigrations";
            this.btnRunMigrations.Size = new System.Drawing.Size(136, 31);
            this.btnRunMigrations.TabIndex = 17;
            this.btnRunMigrations.Text = "RUN Migrations";
            this.btnRunMigrations.UseVisualStyleBackColor = true;
            this.btnRunMigrations.Click += new System.EventHandler(this.btnRunMigrations_Click);
            // 
            // MigrationsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(949, 492);
            this.Controls.Add(this.btnRunMigrations);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.BTNmetadataDelete);
            this.Controls.Add(this.BTNmetadataCopy);
            this.Controls.Add(this.BTNmetadataAdd);
            this.Controls.Add(this.BTNmetadataEdit);
            this.Controls.Add(this.lstMetadataMigrations);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BTNAdd);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lstMigrations);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MigrationsList";
            this.Text = "Camulos Migrator Tool";
            this.Activated += new System.EventHandler(this.Form1_GotFocus);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstMigrations;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button BTNAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileExclusionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderExclusionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metaDataQueriesToolStripMenuItem;
        private System.Windows.Forms.ListBox lstMetadataMigrations;
        private System.Windows.Forms.Button BTNmetadataEdit;
        private System.Windows.Forms.Button BTNmetadataAdd;
        private System.Windows.Forms.Button BTNmetadataCopy;
        private System.Windows.Forms.Button BTNmetadataDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ToolStripMenuItem openMetadataMigrationToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logAnalyserToolStripMenuItem;
        private System.Windows.Forms.Button btnRunMigrations;
    }
}

