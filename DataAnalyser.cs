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
    public partial class DataAnalyser : Form
    {
        public int logmode = 0;
        public DataTable dt;
        List<String> statuses = new List<string>();
        List<String> batches = new List<string>();

        string[] headers = new string[] { "Batchid", "Time", "Status", "SourceFile", "TargetFile", "LogData", "Comments", "Other" };
        string currentFilter = "";
        long currowCount = 0;
        int filteron = 0;
        string savedFileName = "";

        public DataAnalyser()
        {
            InitializeComponent();
            dt = new DataTable();
            this.cboStatusFilter.Enabled = false;
            this.cboBatchFilter.Enabled = false;
            this.btnClearFilter.Enabled = false;
            this.btnApplyFilter.Enabled = false;
            openAddToCurrentToolStripMenuItem.Enabled = false;
            addErrorLogToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            changeChecks(false);
            
        }

        private void changeChecks(Boolean enabled)
        {
            this.chkBatchID.Enabled = enabled;
            this.chkComments.Enabled = enabled;
            this.chkDestination.Enabled = enabled;
            this.chkLogContent.Enabled = enabled;
            this.chkOther.Enabled = enabled;
            this.chkSource.Enabled = enabled;
            this.chkStatus.Enabled = enabled;
            this.chkTime.Enabled = enabled;
            this.btnExport.Enabled = enabled;
        }
        private void DataAnalyser_Load(object sender, EventArgs e)
        {


        }
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
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
                DataTable res = ConvertLogtoDataTable(openFileDialog1.FileName);
                openData(res);
            };
            
        }

        private void openData(DataTable dt)
        {
            this.dataGridView1.DataSource = dt;
            this.toolStripStatusLabel2.Text = String.Format("{0}", dt.Rows.Count);
            currowCount = dt.Rows.Count;
            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;

        }

        private DataTable addToTable(string strFileName)
        {
            if (this.logmode == 0) 
            {
                /// this is log.txt style
               return AddToLogTable(strFileName);
            }
            else
            {
                /// just dump to table single row.
                
                return AddToErrorLogTable(strFileName);
            }
        }
        public DataTable AddToErrorLogTable(string strFilePath)
        {
            changeChecks(false);
            logmode = 1;
            //dt = (this.dataGridView1.DataSource as DataTable);
            StreamReader sr = new StreamReader(strFilePath);
            while (!sr.EndOfStream)
            {
                string row = sr.ReadLine();//.Split(new string[] { "||" }, StringSplitOptions.None);   // Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = row;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public DataTable AddToLogTable(string strFilePath)
        {
            logmode = 0;
            dt = (this.dataGridView1.DataSource as DataTable);
            this.cboStatusFilter.Enabled = true;
            this.cboBatchFilter.Enabled = true;
            this.btnClearFilter.Enabled = true;
            this.btnApplyFilter.Enabled = true;
            StreamReader sr = new StreamReader(strFilePath);
            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split(new string[] { "||" }, StringSplitOptions.None);   // Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    if (rows.Length > i)
                    {
                        dr[i] = rows[i];
                        if (i == 0)
                        {
                            if (!batches.Contains(rows[i]))
                            {
                                batches.Add(rows[i]);
                                this.cboBatchFilter.Items.Add(rows[i]);
                            }
                        }
                        else if (i == 2)
                        {
                            if (!statuses.Contains(rows[i]))
                            {
                                statuses.Add(rows[i]);
                                this.cboStatusFilter.Items.Add(rows[i]);
                            }

                        }
                    }
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
        public DataTable ConvertLogtoDataTable(string strFilePath)
        {
            changeChecks(true);
            logmode = 0;
            statuses = new List<string>();
            batches = new List<string>();
            this.cboStatusFilter.Items.Clear();
            this.cboStatusFilter.Items.Add("All");
            this.cboBatchFilter.Items.Clear();
            this.cboBatchFilter.Items.Add("All");
            this.cboStatusFilter.Enabled = true;
            this.cboBatchFilter.Enabled = true;
            this.btnClearFilter.Enabled = true;
            this.btnApplyFilter.Enabled = true;
            openAddToCurrentToolStripMenuItem.Enabled = true;
            addErrorLogToolStripMenuItem.Enabled = false;

            StreamReader sr = new StreamReader(strFilePath);
            
            //string[] headers = sr.ReadLine().Split(',');
            dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split(new string[] {"||"},StringSplitOptions.None);   // Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    if (rows.Length > i)
                    {
                        dr[i] = rows[i];
                        if (i == 0)
                        {
                            if (!batches.Contains(rows[i]))
                            {
                                batches.Add(rows[i]);
                                this.cboBatchFilter.Items.Add(rows[i]);
                            }
                        }
                        else if (i == 2)
                        {
                            if (!statuses.Contains(rows[i]))
                            {
                                statuses.Add(rows[i]);
                                this.cboStatusFilter.Items.Add(rows[i]);
                            }
                            
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            
            return dt;
        }

        public DataTable ConvertErrorLogtoDataTable(string strFilePath)
        {
            logmode = 1;
            this.cboStatusFilter.Items.Clear();
            this.cboBatchFilter.Items.Clear();

            StreamReader sr = new StreamReader(strFilePath);
            headers = new string[] {"data"};
            //string[] headers = sr.ReadLine().Split(',');
            dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string row = sr.ReadLine();//.Split(new string[] { "||" }, StringSplitOptions.None);   // Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = row;
                }
                dt.Rows.Add(dr);
            }
            this.cboStatusFilter.Enabled = false;
            this.cboBatchFilter.Enabled = false;
            this.btnClearFilter.Enabled = false;
            this.btnApplyFilter.Enabled = false;
            openAddToCurrentToolStripMenuItem.Enabled = false;
            addErrorLogToolStripMenuItem.Enabled = true;

            return dt;
        }

        private void openErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
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
                DataTable res = ConvertErrorLogtoDataTable(openFileDialog1.FileName);
                openData(res);
            };

        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            currentFilter = getFilter();
            (this.dataGridView1.DataSource as DataTable).DefaultView.RowFilter = currentFilter;
            DataTable tb = (this.dataGridView1.DataSource as DataTable).DefaultView.ToTable();
            currowCount = tb.Rows.Count;
            filteron = 1;

        }
        private string getFilter()
        {
            string strfilter = "";
            string strAnd = "";
            if (this.cboStatusFilter.Text != "All" && this.cboStatusFilter.Text.Length > 0)
            {
                strfilter = strfilter + strAnd + string.Format("Status='{0}'", cboStatusFilter.Text);
                strAnd = " AND ";
            }
            if (this.cboBatchFilter.Text != "All" && this.cboBatchFilter.Text.Length > 0)
            {
                strfilter = strfilter + strAnd + string.Format("Batchid='{0}'", cboBatchFilter.Text);
                strAnd = " AND ";
            }
            return strfilter;
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            filteron = 0;
            currentFilter = "";
            (this.dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "";
            currowCount = dt.Rows.Count;
        }

        private void openAddToCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAddToTable();

        }

        private void addErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAddToTable();
        }

        private void doAddToTable()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
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
                DataTable res = addToTable(openFileDialog1.FileName);
                openData(res);
            };

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Text Files",

                CheckFileExists = false,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "Text files (*.csv)|*.csv",
                FilterIndex = 2,
                RestoreDirectory = true,

            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();

                List<String> h = new List<String>();
                int cols = 0;
                List<int> r = new List<int>();
            
                if (chkBatchID.Checked)
                {
                    h.Add(headers[0]);
                    r.Add(0);

                }
                if (chkTime.Checked)
                {
                    h.Add(headers[1]);
                    r.Add(1);
                
                }
                if (chkStatus.Checked)
                {
                    h.Add(headers[2]);
                    r.Add(2);
                }
                if (chkSource.Checked)
                {
                    h.Add(headers[3]);
                    r.Add(3);
                }
                if (chkDestination.Checked)
                {
                    h.Add(headers[4]);
                    r.Add(4);
                }
                if (chkLogContent.Checked)
                {
                    h.Add(headers[5]);
                    r.Add(5);
                }
                if (chkComments.Checked)
                {
                    h.Add(headers[6]);
                    r.Add(6);
                }
                if (chkOther.Checked)
                {
                    h.Add(headers[7]);
                    r.Add(7);
                }

                
                sb.AppendLine(string.Join(",", h));
                DataTable tb = (dataGridView1.DataSource as DataTable);
                DataView dv = tb.DefaultView;
                dv.RowFilter = currentFilter;
                tb = dv.ToTable();

                foreach (DataRow row in tb.Rows)
                {
                    List<string> values = new List<string>();
                    for (int x = 0; x < r.Count; x++)
                    {
                        string s = row[r[x]].ToString();
                        if (s.Contains("\"") || s.Contains(",")){
                            s = s.Replace("\"", "\"\"\"");
                            s = "\"" + s + "\"";
                        }

                        values.Add(s);
                    }
                    sb.AppendLine(string.Join(",", values));
                }

                File.WriteAllText(openFileDialog1.FileName, sb.ToString());

            };

        }

        private void chkBatchID_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBatchID.Checked)
            {
                dataGridView1.Columns[0].Visible = true;
            }
            else
            {
                dataGridView1.Columns[0].Visible = false;
            }
        }

        private void chkTime_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTime.Checked)
            {
                dataGridView1.Columns[1].Visible = true;
            }
            else
            {
                dataGridView1.Columns[1].Visible = false;
            }
        }

        private void chkStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStatus.Checked)
            {
                dataGridView1.Columns[2].Visible = true;
            }
            else
            {
                dataGridView1.Columns[2].Visible = false;
            }
        }

        private void chkSource_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSource.Checked)
            {
                dataGridView1.Columns[3].Visible = true;
            }
            else
            {
                dataGridView1.Columns[3].Visible = false;
            }

        }

        private void chkDestination_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDestination.Checked)
            {
                dataGridView1.Columns[4].Visible = true;
            }
            else
            {
                dataGridView1.Columns[4].Visible = false;
            }

        }

        private void chkLogContent_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLogContent.Checked)
            {
                dataGridView1.Columns[5].Visible = true;
            }
            else
            {
                dataGridView1.Columns[5].Visible = false;
            }

        }

        private void chkComments_CheckedChanged(object sender, EventArgs e)
        {
            if (chkComments.Checked)
            {
                dataGridView1.Columns[6].Visible = true;
            }
            else
            {
                dataGridView1.Columns[6].Visible = false;
            }
        }

        private void chkOther_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOther.Checked)
            {
                dataGridView1.Columns[7].Visible = true;
            }
            else
            {
                dataGridView1.Columns[7].Visible = false;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string filtered = "";
            if (filteron == 1)
            {
                filtered = " (filtered) ";
            }
            toolStripStatusLabel2.Text = string.Format("{0} of {1} {2}", dataGridView1.CurrentCell.RowIndex, currowCount,filtered);
        }

        private void saveFile(Boolean force)
        {
            if (savedFileName.Length > 0 || force)
            {
                SaveFileDialog openFileDialog1 = new SaveFileDialog
                {
                    //InitialDirectory = @"C:\",
                    Title = "Browse Text Files",

                    CheckFileExists = false,
                    CheckPathExists = true,

                    DefaultExt = "txt",
                    Filter = "Text files (*.txt)|*.txt",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                };

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    saveit(openFileDialog1.FileName);
                }
            }
            else
            {
                saveit(savedFileName);
            }
        }
        private void saveit(string filename)
        {
            StringBuilder sb = new StringBuilder();

            //IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
            //                                  Select(column => column.ColumnName);
            //sb.AppendLine(string.Join("||", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join("||", fields));
            }

            File.WriteAllText(filename, sb.ToString());
            savedFileName = filename;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile(false);

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile(true);
        }
    }
}
