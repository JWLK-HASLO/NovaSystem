using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NovaSystem
{
    public partial class MainForm : Form
    {
        //Setting Elemnet
        SerialPort serialPort = new SerialPort();
        private List<byte> PortBuffer = new List<byte>();

        String receiveDataString = null;

        //Array Value
        int columnNumber = 0;
        int rowNumber = 0;

        string[,] dataArrayPressString = null;
        string[,] dataArrayTempString = null;
        int[,] dataArrayValue = null;

        //save Value
        String comboBox_comport_value = Properties.Settings.Default.valueSerailPort;
        String comboBox_layout_value = Properties.Settings.Default.valueSensorLayout;

        /*Value Changed*/
        private void comboBox_comport_selectedValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.valueSerailPort = comboBox_comport.SelectedItem as String;
            Console.WriteLine("Seleted Port Value : {0}", Properties.Settings.Default.valueSerailPort);
        }
        private void comboBox_layout_seletedValueCahnged(object sender, EventArgs e)
        {
            Properties.Settings.Default.valueSensorLayout = comboBox_layout.SelectedItem as String;
            Console.WriteLine("Seleted Layout Value : {0}", Properties.Settings.Default.valueSensorLayout);

            if ((comboBox_layout.SelectedItem as String).Equals("48*48"))
            {
                columnNumber = 48;
                rowNumber = 48;
            }
            else if ((comboBox_layout.SelectedItem as String).Equals("24*14"))
            {
                columnNumber = 24;
                rowNumber = 14;
            }
            dataArrayPressString = setLayoutValueString(columnNumber, rowNumber);
            dataArrayValue = setLayoutValueInt(columnNumber, rowNumber);
        }

        private void FormConnectionLoad()
        {
            /*Port Initialization*/
            comport_initialize(); // This File Inteface_connection.cs

            /*Layout Initialization*/
            comboBox_layout.BeginUpdate();
            Console.WriteLine("Save Port Value : {0}", comboBox_layout_value);
            comboBox_layout.SelectedItem = comboBox_layout_value;

            /*Timer Initialize*/
            timer_initialize(); // Inteface_getData.cs

            /*ColorList Initialize*/
            color_initialize(); // Interface_grid.cs

            /*Grid CLick Event Initialize*/
            gridClickEvent_initialize();

        }


        /* #1-0 Comport Setting */

        private const int DBT_DEVICEARRIVAL = 0x8000;                      // 새로운 Device가 탐지 되었을때    
        private const int DBT_DEVICEREMOVEPENDING = 0x8003;         // Device가 연결 해지 되었을 때    
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        protected override void WndProc(ref Message m)
        {
            //Console.WriteLine("WndProc");
            switch (m.WParam.ToInt32())
            {
                case DBT_DEVICEARRIVAL:
                    Console.WriteLine("Connected");
                    comport_initialize();
                    break;

                case DBT_DEVICEREMOVECOMPLETE:
                    Console.WriteLine("Disconnected");
                    comport_disattatched();
                    break;
            }
            base.WndProc(ref m);
        }

        private void comport_initialize()
        {
            comboBox_comport.BeginUpdate();
            comboBox_comport.Items.Clear();
            comboBox_comport.Items.Add("None");
            foreach (string comport in SerialPort.GetPortNames())
            {
                comboBox_comport.Items.Add(comport);
            }
            comboBox_comport.EndUpdate();
            Console.WriteLine("Save Port Value : {0}", comboBox_comport_value);
            comboBox_comport.SelectedItem = comboBox_comport.Items.Count == 1 ? "None" : comboBox_comport_value;
            CheckForIllegalCrossThreadCalls = false;
        }
        private void comport_disattatched()
        {
            comboBox_comport.BeginUpdate();
            comboBox_comport.Items.Clear();
            comboBox_comport.Items.Add("None");
            foreach (string comport in SerialPort.GetPortNames())
            {
                comboBox_comport.Items.Add(comport);
            }
            comboBox_comport.EndUpdate();
            Console.WriteLine("Save Port Value : {0}", comboBox_comport_value);
            comboBox_comport.SelectedItem = "None";
            CheckForIllegalCrossThreadCalls = false;
        }
        private void button_device_connect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen == false && comboBox_comport.SelectedItem != null && comboBox_comport.SelectedItem.ToString() != "None")
                {
                    /*Serial Port Setting*/
                    serialPort.PortName = comboBox_comport.SelectedItem.ToString();                     //콤보 박스에서 선택.
                    serialPort.BaudRate = int.Parse("1500000".ToString());          //콤보 박스에서 Baud Rate 선택.
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;
                    serialPort.Parity = Parity.None;
                    serialPort.Open();

                    serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler); //데이터 받기.
                    dataLogBox.Text += "연결되었습니다." + Environment.NewLine;

                    serialPort.WriteLine("NovaSystem Connection Check\r\n");                                                    // abcd\r\n Send
                }
                else if (serialPort.IsOpen == false || comboBox_comport.SelectedItem == null || comboBox_comport.SelectedItem.ToString() == "None")
                {
                    MessageBox.Show("기기연결 포트를 선택해 주시기 바랍니다,","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else dataLogBox.Text += "연결되어 있습니다." + Environment.NewLine;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
            
        }
        private void button_device_disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen == true)
                {
                    serialPort.Close();
                    dataLogBox.Text += "해제되었습니다." + Environment.NewLine;
                }
                else dataLogBox.Text += "해제되어 있습니다 ." + Environment.NewLine;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }


        private void button_device_clear_Click(object sender, EventArgs e)
        {

            try
            {
                comboBox_comport.BeginUpdate();
                comboBox_comport.Items.Clear();
                comboBox_comport.Items.Add("None");
                foreach (string comport in SerialPort.GetPortNames())
                {
                    comboBox_comport.Items.Add(comport);
                }
                comboBox_comport.EndUpdate();
                Console.WriteLine("Save Port Value : {0}", comboBox_comport_value);
                comboBox_comport.SelectedItem = "None";
                CheckForIllegalCrossThreadCalls = false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }


        bool resumCounter = false;
        private void button_record_play_Click(object sender, EventArgs e)
        {
            try
            {
                if(serialPort.IsOpen == false)
                {
                    MessageBox.Show("보드를 연결한 상태에서 시도해 주시기 바랍니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox_comport.Focus();
                }
                else
                {
                    if (click_loop == 0 && dataGridView_ScaleControl.Columns.Count != 0)
                    {
                        pannel_import.Enabled = false;
                        pannel_import_timeline.Enabled = false;
                        //Set Text
                        button_record_play.Text = "▍ ▍";
                        click_loop = 1;
                        //Set Timer Setting
                        if (timer == null)
                        {
                            timer_initialize();
                        }
                        timer.Start();
                        //Set StopWatch
                        stopWatch.Start();

                        try
                        {
                            if (resumCounter == false)
                            {
                                streamWriter = new StreamWriter(Application.StartupPath + @"\data\temp_data.txt");
                                resumCounter = true;
                            }
                            else
                            {
                                streamWriter = new StreamWriter(Application.StartupPath + @"\data\temp_data.txt", true);
                            }

                        }
                        catch (Exception)
                        {
                            
                        }
                    }
                    else if (click_loop == 1 && dataGridView_ScaleControl.Columns.Count != 0)
                    {

                        pannel_import.Enabled = true;
                        pannel_import_timeline.Enabled = true;

                        //Set Text
                        button_record_play.Text = "▶";
                        click_loop = 0;
                        //Set Timer Setting
                        timer.Stop();
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
                MessageBox.Show(exception.Message, "Error" ,MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }
        private void button_record_reset_Click(object sender, EventArgs e)
        {
            try
            {
                if (timer.Enabled == false)
                {
                    if (MessageBox.Show("데이터를 초기화 하시겠습니까?", "데이터 초기화", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        /*Save File Reset*/
                        resumCounter = false;
                        streamWriter.Close();
                        /*UI Reset*/
                        setGridClickPosition("none","none");

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
        private void button_record_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (timer.Enabled == false)
                {
                    if (MessageBox.Show("데이터를 저장 하시겠습니까?", "데이터 저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        streamWriter.Flush();
                        streamWriter.Close();
                        SaveFileDialog savefile = new SaveFileDialog();
                        savefile.InitialDirectory = Application.StartupPath + @"\data\";
                        savefile.Title = "파일 저장";
                        savefile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                        savefile.DefaultExt = "txt";
                        savefile.AddExtension = true;

                        if (savefile.ShowDialog() == DialogResult.OK)
                        {
                            File.Move(Application.StartupPath + @"\data\temp_data.txt", savefile.FileName);
                            MessageBox.Show("저장을 완료하였습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("저장하는데 문제가 발생하였습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine("STOP!!!");
                    MessageBox.Show("데이터 스트림을 멈춘 후 다시 시도해 주시기 바랍니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //button_record_play.Focus();
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }

    }
}
