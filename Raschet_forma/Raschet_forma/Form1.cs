using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;

namespace Raschet_forma
{
    public partial class Form1 : Form
    {

        private string fileName = string.Empty;

        private DataTableCollection tableCollection = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();

                if(res == DialogResult.OK)
                {
                    fileName = openFileDialog1.FileName;

                    Text = fileName;

                    OpenExcelFile(fileName);
                }
                else
                {
                    throw new Exception("Файл не выбран!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenExcelFile(string path)
        {
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
           
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

            DataSet db = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (x) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            tableCollection = db.Tables;

            toolStripComboBox1.Items.Clear();

            foreach (DataTable tabe in tableCollection)
            {
                toolStripComboBox1.Items.Add(tabe.TableName);
            }

            toolStripComboBox1.SelectedIndex = 0;

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable table = tableCollection[Convert.ToString(toolStripComboBox1.SelectedItem)];

            dataGridView1.DataSource = table;
        }
    }
}
