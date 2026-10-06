namespace CamulosSharePointUpload
{
    partial class DataAnalyser
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cboStatusFilter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openAddToCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openErrorLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.cboBatchFilter = new System.Windows.Forms.ComboBox();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.addErrorLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkBatchID = new System.Windows.Forms.CheckBox();
            this.chkTime = new System.Windows.Forms.CheckBox();
            this.chkStatus = new System.Windows.Forms.CheckBox();
            this.chkSource = new System.Windows.Forms.CheckBox();
            this.chkDestination = new System.Windows.Forms.CheckBox();
            this.chkComments = new System.Windows.Forms.CheckBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.chkLogContent = new System.Windows.Forms.CheckBox();
            this.chkOther = new System.Windows.Forms.CheckBox();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(1, 130);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1232, 489);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 622);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1234, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1234, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openAddToCurrentToolStripMenuItem,
            this.openErrorLogToolStripMenuItem,
            this.addErrorLogToolStripMenuItem,
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
            this.openToolStripMenuItem.Text = "Open Results File";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // cboStatusFilter
            // 
            this.cboStatusFilter.FormattingEnabled = true;
            this.cboStatusFilter.Location = new System.Drawing.Point(73, 45);
            this.cboStatusFilter.Name = "cboStatusFilter";
            this.cboStatusFilter.Size = new System.Drawing.Size(121, 21);
            this.cboStatusFilter.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Status";
            // 
            // openAddToCurrentToolStripMenuItem
            // 
            this.openAddToCurrentToolStripMenuItem.Name = "openAddToCurrentToolStripMenuItem";
            this.openAddToCurrentToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openAddToCurrentToolStripMenuItem.Text = "Add Results File";
            this.openAddToCurrentToolStripMenuItem.Click += new System.EventHandler(this.openAddToCurrentToolStripMenuItem_Click);
            // 
            // openErrorLogToolStripMenuItem
            // 
            this.openErrorLogToolStripMenuItem.Name = "openErrorLogToolStripMenuItem";
            this.openErrorLogToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openErrorLogToolStripMenuItem.Text = "Open Simple Log";
            this.openErrorLogToolStripMenuItem.Click += new System.EventHandler(this.openErrorLogToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Batch";
            // 
            // cboBatchFilter
            // 
            this.cboBatchFilter.FormattingEnabled = true;
            this.cboBatchFilter.Location = new System.Drawing.Point(73, 73);
            this.cboBatchFilter.Name = "cboBatchFilter";
            this.cboBatchFilter.Size = new System.Drawing.Size(315, 21);
            this.cboBatchFilter.TabIndex = 6;
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Location = new System.Drawing.Point(73, 101);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(121, 23);
            this.btnApplyFilter.TabIndex = 7;
            this.btnApplyFilter.Text = "Filter";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApplyFilter_Click);
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Location = new System.Drawing.Point(200, 101);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(121, 23);
            this.btnClearFilter.TabIndex = 8;
            this.btnClearFilter.Text = "Clear Filter";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(587, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Export results";
            // 
            // addErrorLogToolStripMenuItem
            // 
            this.addErrorLogToolStripMenuItem.Name = "addErrorLogToolStripMenuItem";
            this.addErrorLogToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addErrorLogToolStripMenuItem.Text = "Add Simple Log";
            this.addErrorLogToolStripMenuItem.Click += new System.EventHandler(this.addErrorLogToolStripMenuItem_Click);
            // 
            // chkBatchID
            // 
            this.chkBatchID.AutoSize = true;
            this.chkBatchID.Checked = true;
            this.chkBatchID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBatchID.Location = new System.Drawing.Point(590, 48);
            this.chkBatchID.Name = "chkBatchID";
            this.chkBatchID.Size = new System.Drawing.Size(65, 17);
            this.chkBatchID.TabIndex = 10;
            this.chkBatchID.Text = "BatchID";
            this.chkBatchID.UseVisualStyleBackColor = true;
            this.chkBatchID.CheckedChanged += new System.EventHandler(this.chkBatchID_CheckedChanged);
            // 
            // chkTime
            // 
            this.chkTime.AutoSize = true;
            this.chkTime.Checked = true;
            this.chkTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTime.Location = new System.Drawing.Point(590, 71);
            this.chkTime.Name = "chkTime";
            this.chkTime.Size = new System.Drawing.Size(49, 17);
            this.chkTime.TabIndex = 11;
            this.chkTime.Text = "Time";
            this.chkTime.UseVisualStyleBackColor = true;
            this.chkTime.CheckedChanged += new System.EventHandler(this.chkTime_CheckedChanged);
            // 
            // chkStatus
            // 
            this.chkStatus.AutoSize = true;
            this.chkStatus.Checked = true;
            this.chkStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStatus.Location = new System.Drawing.Point(590, 94);
            this.chkStatus.Name = "chkStatus";
            this.chkStatus.Size = new System.Drawing.Size(56, 17);
            this.chkStatus.TabIndex = 12;
            this.chkStatus.Text = "Status";
            this.chkStatus.UseVisualStyleBackColor = true;
            this.chkStatus.CheckedChanged += new System.EventHandler(this.chkStatus_CheckedChanged);
            // 
            // chkSource
            // 
            this.chkSource.AutoSize = true;
            this.chkSource.Checked = true;
            this.chkSource.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSource.Location = new System.Drawing.Point(688, 48);
            this.chkSource.Name = "chkSource";
            this.chkSource.Size = new System.Drawing.Size(79, 17);
            this.chkSource.TabIndex = 13;
            this.chkSource.Text = "Source File";
            this.chkSource.UseVisualStyleBackColor = true;
            this.chkSource.CheckedChanged += new System.EventHandler(this.chkSource_CheckedChanged);
            // 
            // chkDestination
            // 
            this.chkDestination.AutoSize = true;
            this.chkDestination.Checked = true;
            this.chkDestination.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDestination.Location = new System.Drawing.Point(688, 71);
            this.chkDestination.Name = "chkDestination";
            this.chkDestination.Size = new System.Drawing.Size(79, 17);
            this.chkDestination.TabIndex = 14;
            this.chkDestination.Text = "Destination";
            this.chkDestination.UseVisualStyleBackColor = true;
            this.chkDestination.CheckedChanged += new System.EventHandler(this.chkDestination_CheckedChanged);
            // 
            // chkComments
            // 
            this.chkComments.AutoSize = true;
            this.chkComments.Checked = true;
            this.chkComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkComments.Location = new System.Drawing.Point(794, 48);
            this.chkComments.Name = "chkComments";
            this.chkComments.Size = new System.Drawing.Size(75, 17);
            this.chkComments.TabIndex = 16;
            this.chkComments.Text = "Comments";
            this.chkComments.UseVisualStyleBackColor = true;
            this.chkComments.CheckedChanged += new System.EventHandler(this.chkComments_CheckedChanged);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(902, 63);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 16;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // chkLogContent
            // 
            this.chkLogContent.AutoSize = true;
            this.chkLogContent.Checked = true;
            this.chkLogContent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLogContent.Location = new System.Drawing.Point(688, 94);
            this.chkLogContent.Name = "chkLogContent";
            this.chkLogContent.Size = new System.Drawing.Size(84, 17);
            this.chkLogContent.TabIndex = 15;
            this.chkLogContent.Text = "Log Content";
            this.chkLogContent.UseVisualStyleBackColor = true;
            this.chkLogContent.CheckedChanged += new System.EventHandler(this.chkLogContent_CheckedChanged);
            // 
            // chkOther
            // 
            this.chkOther.AutoSize = true;
            this.chkOther.Checked = true;
            this.chkOther.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOther.Location = new System.Drawing.Point(794, 73);
            this.chkOther.Name = "chkOther";
            this.chkOther.Size = new System.Drawing.Size(52, 17);
            this.chkOther.TabIndex = 17;
            this.chkOther.Text = "Other";
            this.chkOther.UseVisualStyleBackColor = true;
            this.chkOther.CheckedChanged += new System.EventHandler(this.chkOther_CheckedChanged);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(52, 17);
            this.toolStripStatusLabel1.Text = "Records:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(66, 17);
            this.toolStripStatusLabel2.Text = "Select a file";
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
            // DataAnalyser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 644);
            this.Controls.Add(this.chkOther);
            this.Controls.Add(this.chkLogContent);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.chkComments);
            this.Controls.Add(this.chkDestination);
            this.Controls.Add(this.chkSource);
            this.Controls.Add(this.chkStatus);
            this.Controls.Add(this.chkTime);
            this.Controls.Add(this.chkBatchID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnClearFilter);
            this.Controls.Add(this.btnApplyFilter);
            this.Controls.Add(this.cboBatchFilter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboStatusFilter);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dataGridView1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DataAnalyser";
            this.Text = "DataAnalyser";
            this.Load += new System.EventHandler(this.DataAnalyser_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openAddToCurrentToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboStatusFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem openErrorLogToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboBatchFilter;
        private System.Windows.Forms.Button btnApplyFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem addErrorLogToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkBatchID;
        private System.Windows.Forms.CheckBox chkTime;
        private System.Windows.Forms.CheckBox chkStatus;
        private System.Windows.Forms.CheckBox chkSource;
        private System.Windows.Forms.CheckBox chkDestination;
        private System.Windows.Forms.CheckBox chkComments;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox chkLogContent;
        private System.Windows.Forms.CheckBox chkOther;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
    }
}