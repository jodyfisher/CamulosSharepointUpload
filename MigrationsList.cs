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
    

    public partial class MigrationsList : Form
    {
        public string currentFileName = "";
        public MigrationsList()
        {
            InitializeComponent();
            this.GotFocus += new System.EventHandler(this.Form1_GotFocus);
            Configuration.Migrationdb = new MigrationsDatabase(); //configuration.LoadMigrationDatabase(configuration.migrationsdatafile);
        }

        private void BTNAdd_Click(object sender, EventArgs e)
        {
            FRMMigration F = new FRMMigration();

            F.parentForm1 = this;
            F.Show();
        }

        public void refreshLists()
        {
            this.lstMigrations.Items.Clear();
            this.loadData();
            
        }
        private void loadData()
        {
            for (int i = 0; i < Configuration.Migrationdb.Migrations.Count; i++)
            {
                lstMigrations.Items.Add(Configuration.Migrationdb.Migrations[i].Reference);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //lstMigrations.Items.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "spm",
                Filter = "camulos sp migrator files (*.spm)|*.spm",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lstMigrations.Items.Clear();
                Configuration.migrationsdatafile = openFileDialog1.FileName;
                Configuration.Migrationdb = Configuration.LoadMigrationDatabase(Configuration.migrationsdatafile);
                this.loadData();
            };
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
            /*SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = false,
                CheckPathExists = true,

                DefaultExt = "spm",
                Filter = "camulos sp migrator files (*.spm)|*.spm",
                FilterIndex = 2,
                RestoreDirectory = true,

                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                configuration.migrationsdatafile = saveFileDialog1.FileName;
                configuration.Migrationdb.Save(configuration.migrationsdatafile);
            };*/
        }

        private void lstMigrations_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        private void openMigration(object sender, EventArgs e)
        {
            FRMMigration F = new FRMMigration();
            F.migration = Configuration.Migrationdb.Migrations[lstMigrations.SelectedIndex];
            F.parentForm1 = this;
            F.loadData();
            F.Show();
        }

        private void fileExclusionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRMExclusionsEditor F = new FRMExclusionsEditor();
            F.Show();
        }

        private void folderExclusionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRMFolderExclusionsEditor F = new FRMFolderExclusionsEditor();
            F.Show();
        }

        private void metaDataQueriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRMMetaDataEditor F = new FRMMetaDataEditor();
            F.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lstMigrations.Items.RemoveAt(lstMigrations.SelectedIndex);
        }

        private void saveData(string fileName)
        {
            Configuration.Migrationdb.Save(fileName);
            /*
            string[] lines = new string[lstMigrations.Items.Count];
            int itemCount = lstMigrations.Items.Count;
            int i = 0;
            foreach (string item in lstMigrations.Items)
            {
                lines[i] = item.ToString();
                i = i + 1;
            }
            System.IO.File.WriteAllLines(fileName, lines);
            */
        }

        private void saveAsFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = false,
                CheckPathExists = true,

                DefaultExt = "spm",
                Filter = "camulos sp migrator files (*.spm)|*.spm",
                FilterIndex = 2,
                RestoreDirectory = true,

                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.currentFileName = saveFileDialog1.FileName;
                saveData(this.currentFileName);
            };
        }

        private void saveFile()
        {
            if (this.currentFileName == "")
            {
                saveAsFile();
            }
            else
            {
                saveData(this.currentFileName);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsFile();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void Form1_GotFocus(object sender, EventArgs e)
        {
            //Console.WriteLine("Activated");
        }





        //metadata migrator lst stuff

        private void BTNmetadataEdit_Click(object sender, EventArgs e)
        {

        }

        private void BTNmetadataAdd_Click(object sender, EventArgs e)
        {
            FRMMetadataMigration F = new FRMMetadataMigration();

            F.parentForm1 = this;
            F.Show();
        }

        private void BTNmetadataDelete_Click(object sender, EventArgs e)
        {
            lstMetadataMigrations.Items.RemoveAt(lstMetadataMigrations.SelectedIndex);
        }

        private void loadMetaData()
        {
            for (int i = 0; i < Configuration.Migrationdb.Migrations.Count; i++)
            {
                lstMetadataMigrations.Items.Add(Configuration.Migrationdb.MetadataMigrations[i].ReferenceMeta);
            }
        }

        private void openMetadataMigrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "camulos meta data migrator files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lstMetadataMigrations.Items.Clear();
                Configuration.migrationsdatafile = openFileDialog1.FileName;
                Configuration.Migrationdb = Configuration.LoadMigrationDatabase(Configuration.migrationsdatafile);
                this.loadMetaData();
            };
        }

        private void lstMetadataMigrations_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void lstMetadataMigrations_DoubleClick(object sender, EventArgs e)
        {
            FRMMetadataMigration F = new FRMMetadataMigration();
            F.metadataMigration = Configuration.Migrationdb.MetadataMigrations[lstMetadataMigrations.SelectedIndex];
            F.parentForm1 = this;
            F.loadMetaData();
            F.Show();
        }

        private void logAnalyserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataAnalyser f = new DataAnalyser();
            f.Show();
        }

        private void btnRunMigrations_Click(object sender, EventArgs e)
        {
            foreach(Migration m in Configuration.Migrationdb.Migrations)
            {
                Configuration.o365List = m.DocLibraryName;
                Configuration.o365Password = m.Password;
                Configuration.o365UserName = m.Username;
                Configuration.o365SiteURL = m.SharepointSite;
                Configuration.listGUID = m.DocLibraryGUiD;
                Configuration.localSource = m.DocSource;
                Configuration.startfrom = m.Resume;
                Configuration.excludeFileName = m.ExcludeFiles;
                Configuration.excludeFolderFileName = m.ExcludeFolders;
                //Configuration.timeoutValue = Int32.Parse(m.Timeout);
                Configuration.logFileName = m.LogFileName;
                Configuration.errorFileName = m.ErrorFileName;
                Program.StartMigrationFromForm();




            }
        }


















        /////
    }
}
