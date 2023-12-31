﻿using System;
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
        private void PerformCalculations(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                object valueAObj = row["Значение 1"];
                object valueBObj = row["Значение 2"];

                if (valueAObj != DBNull.Value && valueBObj != DBNull.Value)
                {
                    double valueA = Convert.ToDouble(valueAObj);
                    double valueB = Convert.ToDouble(valueBObj);

                    double result = valueB / valueA; // Ваш расчет

                    // Запись результата в столбец "Результат"
                    row["Результат"] = result;
                }
                else
                {
                    row["Результат"] = DBNull.Value; // При отсутствии данных записываем DBNull
                }
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

            foreach (DataTable table in tableCollection)
            {
                if (table.TableName == "Лист1") // Проверяем, что это нужный лист
                {
                    // Добавляем новую пустую строку в начало таблицы
                    DataRow emptyRow = table.NewRow();
                    table.Rows.InsertAt(emptyRow, 0);

                    // Добавляем столбец "Результат" в конец таблицы
                    table.Columns.Add("Результат", typeof(double));

                    // Выполняем расчеты
                    PerformCalculations(table);

                    toolStripComboBox1.Items.Add(table.TableName);
                    toolStripComboBox1.SelectedIndex = 0;
                }
                else
                {
                    toolStripComboBox1.Items.Add(table.TableName);
                }
            }


        }



        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable table = tableCollection[Convert.ToString(toolStripComboBox1.SelectedItem)];
            PerformCalculations(table);

            dataGridView1.DataSource = table;
        }
    }
}
