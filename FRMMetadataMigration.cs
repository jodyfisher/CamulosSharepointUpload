using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamulosSharePointUpload
{
    public partial class FRMMetadataMigration : Form
    {
        public string metadataMigrationID = "";
        public MetadataMigration metadataMigration;
        public MigrationsList parentForm1;

        public FRMMetadataMigration()
        {
            InitializeComponent();
            this.metadataMigration = new MetadataMigration();

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        public void loadMetaData()
        {
            TXTreference.Text = this.metadataMigration.ReferenceMeta;
            TXTsharepointSite.Text = this.metadataMigration.SharepointSiteMeta;
            TXTusername.Text = this.metadataMigration.UsernameMeta;
            TXTpassword.Text = this.metadataMigration.PasswordMeta;
            TXTlibrary.Text = this.metadataMigration.LibraryMeta;
            TXTqueryFile.Text = this.metadataMigration.QueryFileMeta;
        }

        private void BTNsave_Click(object sender, EventArgs e)
        {
            this.metadataMigration.ReferenceMeta = TXTreference.Text;
            this.metadataMigration.SharepointSiteMeta = TXTsharepointSite.Text;
            this.metadataMigration.UsernameMeta = TXTusername.Text;
            this.metadataMigration.PasswordMeta = TXTpassword.Text;
            this.metadataMigration.LibraryMeta = TXTlibrary.Text;
            this.metadataMigration.QueryFileMeta = TXTqueryFile.Text;

            if (this.metadataMigration.IDMeta.Length == 0)
            {
                /// new record
                this.metadataMigration.IDMeta = Guid.NewGuid().ToString();
                Configuration.Migrationdb.InsertRecordMetadata(this.metadataMigration);
            }
            else
            {
                // existing record
                Configuration.Migrationdb.UpdateRecordMetadata(this.metadataMigration);
            }
        }
    }
}
