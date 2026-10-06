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
using System.Text.RegularExpressions;

namespace CamulosSharePointUpload
{
    public partial class FRMExclusionsEditor : Form
    {
        public string currentFileName = "";
        public FRMExclusionsEditor()
        {
            InitializeComponent();
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
                    lstExclusions.Items.Add(line);
                }
            }
        }

        private void saveData(string fileName)
        {
            string[] lines = new string[lstExclusions.Items.Count];
            int itemCount = lstExclusions.Items.Count;
            int i = 0;
            foreach (string item in lstExclusions.Items)
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstExclusions.Items.Clear();
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
            TestSectionVisible();
        }

        private void TXTlineItem_TextChanged(object sender, EventArgs e)
        {
           // TXTlineItem.Text = lstExclusions.SelectedItem.Text;
        }

        private void lstExclusions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstExclusions.SelectedIndex == -1)
            {
                TXTlineItem.Text = "";
            }
            else
            {
                string itemData = lstExclusions.SelectedItem.ToString();
                string str = itemData.Substring(0, 4);
                if (str == "[re]")
                {
                    comboBox1.SelectedIndex = 1;
                    TXTlineItem.Text = lstExclusions.SelectedItem.ToString().Substring(5);
                    string input = TXTegFileName.Text;
                }
                else
                {
                    comboBox1.SelectedIndex = 0;
                    TXTlineItem.Text = lstExclusions.SelectedItem.ToString();
                }
            }
            TestSectionVisible();
        }

        private void BTNupdate_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                lstExclusions.Items[lstExclusions.SelectedIndex] = "[re]\\" + TXTlineItem.Text;
            }
            else
            {
                lstExclusions.Items[lstExclusions.SelectedIndex] = TXTlineItem.Text;
            }
        }

        private void TestSectionVisible()
        {
            if(comboBox1.SelectedIndex == 1)
            {
                label1.Visible = true;
                label2.Visible = true;
                TXTegFileName.Visible = true;
                label3.Visible = true;
                BTNtest.Visible = true;
            }
            else
            {
                label1.Visible = false;
                label2.Visible = false;
                TXTegFileName.Visible = false;
                label3.Visible = false;
                BTNtest.Visible = false;
            }
        }

        private void BTNsaveFile_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void MNUsave_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void MNUsaveAs_Click(object sender, EventArgs e)
        {
            saveAsFile();
        }

        private void BTNtest_Click(object sender, EventArgs e)
        {
            var regex = TXTlineItem.Text;
            string input = TXTegFileName.Text;

            var match = Regex.Match(input, regex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // does not match
                label3.Text = "No Match";
                label3.BackColor = Color.LightSalmon;
                
            }
            else
            {
                label3.Text = "Match";
                
                label3.BackColor = Color.Green;
            }
        }

        private void BTNadd_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                lstExclusions.Items.Add("[re]\\" + TXTlineItem.Text);
            }
            else
            {
                lstExclusions.Items.Add(TXTlineItem.Text);
            }
            
        }

        private void BTNdelete_Click(object sender, EventArgs e)
        {
            lstExclusions.Items.RemoveAt(lstExclusions.SelectedIndex);
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
    }
}
