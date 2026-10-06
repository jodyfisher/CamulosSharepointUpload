using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CamulosSharePointUpload
{
    public partial class FRMMigration : Form
    {
        public string migrationid = "";
        public Migration migration;
        public MigrationsList parentForm1;
        public FRMMigration()
        {
            InitializeComponent();
            this.migration = new Migration();
            /*if (this.migrationid.Length > 0)
            {
                //// load the migration
                this.migration = configuration.Migrationdb.GetRecord(this.migrationid);

            }
            else
            {
                this.migration = new Migration();
            }
            TXTReference.Text = this.migration.Reference;
            TXTSharepointSite.Text = this.migration.SharepointSite;
            TXTUsername.Text = this.migration.Username;
            TXTPassword.Text = this.migration.Password;
            TXTDocLibraryName.Text = this.migration.DocLibraryName;
            TXTDocLibraryGUiD.Text = this.migration.DocLibraryGUiD;
            TXTDocSource.Text = this.migration.DocSource;
            TXTStartFolder.Text = this.migration.StartFolder;
            TXTResume.Text = this.migration.Resume;
            TXTTimeout.Text = this.migration.Timeout;
            TXTMetadataUpdate.Text = this.migration.MetadataUpdate;
            */
        }
        public void loadData()
        {
            TXTReference.Text = this.migration.Reference;
            TXTSharepointSite.Text = this.migration.SharepointSite;
            TXTUsername.Text = this.migration.Username;
            TXTPassword.Text = this.migration.Password;
            TXTDocLibraryName.Text = this.migration.DocLibraryName;
            TXTDocLibraryGUiD.Text = this.migration.DocLibraryGUiD;
            TXTDocSource.Text = this.migration.DocSource;
            TXTStartFolder.Text = this.migration.StartFolder;
            TXTResume.Text = this.migration.Resume;
            TXTTimeout.Text = this.migration.Timeout;
            TXTlogFileName.Text = this.migration.LogFileName;
            TXTerrorFileName.Text = this.migration.ErrorFileName;
            TXTexcludeFiles.Text = this.migration.ExcludeFiles;
            TXTexcludeFolders.Text = this.migration.ExcludeFolders;
        }

        private void BTNSave_Click(object sender, EventArgs e)
        {


            this.migration.Reference = TXTReference.Text;
            this.migration.SharepointSite = TXTSharepointSite.Text;
            this.migration.Username = TXTUsername.Text;
            this.migration.Password = TXTPassword.Text;
            this.migration.DocLibraryName = TXTDocLibraryName.Text;
            this.migration.DocLibraryGUiD = TXTDocLibraryGUiD.Text;
            this.migration.DocSource = TXTDocSource.Text;
            this.migration.StartFolder = TXTStartFolder.Text;
            this.migration.Resume = TXTResume.Text;
            this.migration.Timeout = TXTTimeout.Text;
            this.migration.LogFileName = TXTlogFileName.Text;
            this.migration.ErrorFileName = TXTerrorFileName.Text;
            this.migration.ExcludeFiles = TXTexcludeFiles.Text;
            this.migration.ExcludeFolders = TXTexcludeFolders.Text;
            
            //// Up to here Max
            ///add all the relevant TXT fields to the migration object here.


            if (this.migration.ID.Length == 0)
            {
                /// new record
                this.migration.ID = Guid.NewGuid().ToString();
                Configuration.Migrationdb.InsertRecord(this.migration);
            }
            else
            {
                // existing record
                Configuration.Migrationdb.UpdateRecord(this.migration);
            }

            this.parentForm1.refreshLists();
            this.Close();

            //configuration.Migrationdb.Save(configuration.migrationsdatafile);
            
        }

        private void BTNexFilesBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                TXTexcludeFiles.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void BTNexFoldersBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                TXTexcludeFolders.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
