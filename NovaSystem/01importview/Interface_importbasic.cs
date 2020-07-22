using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NovaSystem
{
    public partial class MainForm : Form
    {
        StreamReader streamReader;
        bool readerTrigger = false;
        private void button_loadExistRecord_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Application.StartupPath + @"\data\";
            openFileDialog.Title = "txt 파일 불러오기";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                streamReader = new StreamReader(openFileDialog.FileName);
                textBox_dataPath.Text = openFileDialog.FileName;
                readerTrigger = true;
            }
        }

        int import_click_loop = 0;

        private void button_import_play_Click(object sender, EventArgs e)
        {
            try
            {
                if (!readerTrigger)
                {
                    MessageBox.Show("파일을 불러온 후에 시도해 주시기 바랍니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    button_loadExistRecord.Focus();
                }
                else
                {
                    if (import_click_loop == 0 && dataGridView_ScaleControl.Columns.Count != 0)
                    {
                        panel_record_controller.Enabled = false;
                        //Set Text
                        button_import_play.Text = "▍ ▍";
                        import_click_loop = 1;
                        //Set Timer Setting
                        if (timer == null)
                        {
                            timer_initialize();
                        }
                        if (timerReadData == null)
                        {
                            timer_read_initialize();
                        }
                        timer.Start();
                        timerReadData.Start();
                        //Set StopWatch
                        stopWatch.Start();

                        
                    }
                    else if (import_click_loop == 1 && dataGridView_ScaleControl.Columns.Count != 0)
                    {
                        panel_record_controller.Enabled = true;
                        //Set Text
                        button_import_play.Text = "▶";
                        import_click_loop = 0;
                        //Set Timer Setting
                        timer.Stop();
                        timerReadData.Stop();
                        //Set StopWatch
                        stopWatch.Stop();
                    }
                    else
                    {
                        MessageBox.Show("센서 레이아웃 설정 후 다시 시도해 주시기 바랍니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        button_layout.Focus();
                    }
                }


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }

        private void button_import_reset_Click(object sender, EventArgs e)
        {
            try
            {
                if (timer.Enabled == false)
                {
                    if (MessageBox.Show("데이터를 초기화 하시겠습니까?", "데이터 초기화", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        /*Save File Reset*/
                        resumCounter = false;
                        streamReader.Close();
                        /*UI Reset*/
                        setGridClickPosition("none", "none");

                        /*Data Reset*/
                        dataGridView_ScaleControl.Columns.Clear();
                        dataArrayPressString = setLayoutValueString(columnNumber, rowNumber);

                        /*TImer Reset*/
                        timer.Close();
                        stopWatch.Reset();
                        timSpan = new TimeSpan(0, 0, 0, 0);
                        dataIndex = 0;
                        textBox_timer.Text = String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);
                        textBox_timer_import.Text = String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);

                        MessageBox.Show("초기화 되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
                else MessageBox.Show("데이터 스트림을 멈춘 후 다시 시도해 주시기 바랍니다.");


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }

        private void button_import_save_Click(object sender, EventArgs e)
        {
            MessageBox.Show("업데이트 예정입니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //private datatable getdatatablefromcsv(string path, bool isfirstrowheader)
        //{
        //    string header = isfirstrowheader ? "yes" : "no";
        //    string pathonly = path.getdirectoryname(path);
        //    string filename = path.getfilename(path);
        //    string sql = @"select * from [" + filename + "]";
        //    using (oledbconnection connection = new oledbconnection(
        //        @"provider=microsoft.jet.oledb.4.0;data source=" + pathonly + ";extended properties=\"text;hdr=" + header + "\""))
        //    using (oledbcommand command = new oledbcommand(sql, connection))
        //    using (oledbdataadapter adapter = new oledbdataadapter(command))
        //    {
        //        datatable datatable = new datatable();
        //        datatable.locale = cultureinfo.currentculture;
        //        adapter.fill(datatable);
        //        return datatable;
        //    }
        //}
    }
}
