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
using System.Data.OleDb;
using System.Globalization;

namespace NovaSystem
{
    public partial class LayoutForm : Form
    {
        public LayoutForm()
        {
            InitializeComponent();
        }

        private void LayoutForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine(this.Text);
            string formtext = "New Data";
            if (!this.Text.Contains(formtext))
            {
                textBox_dataPath.Text = this.Text;
            }
        }

        private void button_dataImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Title = "CSV 파일 불러오기";
            openFileDialog.Filter = "CSV 파일 (*.csv)|*.csv|모든 파일 (*.*)|*.*";

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                DataTable newLoadDataTable = new DataTable();

                string FileName = openFileDialog.FileName;
                textBox_dataPath.Text = FileName;
                DataTable dt1 = getDataTableFromCsv(FileName, false);
                dataGridView_dataList.DataSource = dt1;

                DataRow firstData = dt1.Rows[0];
                firstData.Delete();
                
                string[] data = new string[5];

            }
            
            
        }

        private DataTable getDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";
            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            string sql = @"SELECT * FROM [" + fileName + "]";
            using (OleDbConnection connection = new OleDbConnection(
                @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly + ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    
    }
}
