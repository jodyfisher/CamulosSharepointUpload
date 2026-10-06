using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace CamulosSharePointUpload
{
    public partial class FRMMetaDataEditor : Form
    {
        public string currentFileName = "";
        public FRMMetaDataEditor()
        {
            InitializeComponent();
            for (int i = 0; i < Configuration.Migrationdb.Migrations.Count; i++)
            {
                
                comboBox1.Items.Add(Configuration.Migrationdb.Migrations[i].Reference);
            }
        }

        private void lstMetadataEditor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMetadataEditor.SelectedIndex == -1)
            {
               // TXTlineItem.Text = "";
            }
            else
            {
                string itemData = lstMetadataEditor.SelectedItem.ToString();
                string str = itemData.Substring(0, 4);
                if (itemData.Contains(","))
                {
                    string[] items = itemData.Split(',');
                    if(items.Length > 2)
                    {
                        TXTpath.Text = items[0];
                        TXTupdate.Text = items[1];
                        TXTvalue.Text = items[2];
                    }
                    else
                    {
                        TXTpath.Text = itemData;
                        TXTupdate.Text = "";
                        TXTvalue.Text = "";
                    }
                }
                else
                {
                    TXTpath.Text = itemData;
                    TXTupdate.Text = "";
                    TXTvalue.Text = "";
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstMetadataEditor.Items.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "Text files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.loadData(openFileDialog1.FileName);
            };
        }

        private void loadData(string fileName)
        {
            if (File.Exists(fileName))
            {
                
                this.currentFileName = fileName;
                string text = File.ReadAllText(fileName);


                // Read a text file line by line.
                string[] lines = File.ReadAllLines(fileName);
                foreach (string line in lines)
                {
                    lstMetadataEditor.Items.Add(line);
                }
            }
        }

        private void saveData(string fileName)
        {
            string[] lines = new string[lstMetadataEditor.Items.Count];
            int itemCount = lstMetadataEditor.Items.Count;
            int i = 0;
            foreach (string item in lstMetadataEditor.Items)
            {
                lines[i] = item.ToString();
                i = i + 1;
            }
            System.IO.File.WriteAllLines(fileName, lines);
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

        private void saveAsFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = false,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "Text files (*.txt)|*.txt",
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

        private void BTNupdate_Click(object sender, EventArgs e)
        {
            lstMetadataEditor.Items[lstMetadataEditor.SelectedIndex] = TXTpath.Text + "," + TXTupdate.Text + "," + TXTvalue.Text;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAsFile();
        }

        private void BTNadd_Click(object sender, EventArgs e)
        {
            lstMetadataEditor.Items.Add(TXTpath.Text + "," + TXTupdate.Text + "," + TXTvalue.Text);
        }

        private void BTNdelete_Click(object sender, EventArgs e)
        {
            lstMetadataEditor.Items.RemoveAt(lstMetadataEditor.SelectedIndex);
        }

        private void BTNsaveFile_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void addFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "Text files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.loadData(openFileDialog1.FileName);
            };
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
