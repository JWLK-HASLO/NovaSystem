using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NovaSystem1._0
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
            comboBox_comport.BeginUpdate();
            foreach (string comport in SerialPort.GetPortNames())
            {
                comboBox_comport.Items.Add(comport);
            }
            comboBox_comport.EndUpdate();
            Console.WriteLine("Save Port Value : {0}", comboBox_comport_value);
            comboBox_comport.SelectedItem = comboBox_comport_value;
            CheckForIllegalCrossThreadCalls = false;

            /*Layout Initialization*/
            comboBox_layout.BeginUpdate();
            Console.WriteLine("Save Port Value : {0}", comboBox_layout_value);
            comboBox_layout.SelectedItem = comboBox_layout_value;

            /*Timer Initialize*/
            timer_initialize();

            /*ColorList Initialize*/
            color_initialize();

        }


        /* #1-0 Comport Setting */
        private void button_device_connect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen == false)
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

        /* #1-1 Data Receive*/
        string[] dataSaverString = new string[2];
        bool trigger = false;
        int dataCounter = 0;
        int dataPriority = 0;
        int dataColLineNumber = 0;
        int dataRowLineNumber = 0;
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort portSender = (SerialPort)sender;

                /*Rade Data Byte*/
                byte[] byteBuffer = new byte[portSender.BytesToRead];
                portSender.Read(byteBuffer, 0, byteBuffer.Length);
                //PortBuffer.AddRange(byteBuffer);\
                //Console.WriteLine(byte.Join(",", byteBuffer);

                /* */
                for (int iTemp = 0; iTemp < byteBuffer.Length; iTemp++)
                {
                    receiveDataString = byteBuffer[iTemp].ToString("X2");
                    //Console.WriteLine("Data[{0:00}] HEX: {1}  ", DataBufferTiming, receiveDataString);
                    
                    if(receiveDataString == "FC")
                    {
                        dataSaverString[0] = receiveDataString;
                    }

                    if(dataSaverString[0] == "FC" && receiveDataString =="FD")
                    {
                        //Console.WriteLine("Data Start");
                        dataSaverString[1] = receiveDataString;
                    }

                    if(dataSaverString[1] == "FD" && receiveDataString == "50")
                    {
                        dataSaverString[2] = receiveDataString;
                        trigger = true;
                    }

                    if(dataSaverString[2] == "50" && trigger == true)
                    {
                        if(dataCounter == 0)
                        {
                            //Console.WriteLine("Data Type Press");
                        }
                        else if (dataCounter == 1)
                        {
                            //Console.WriteLine("Data Column Number  : {0}", Int64.Parse(receiveDataString, System.Globalization.NumberStyles.HexNumber));
                        }
                        else if (dataCounter == 2)
                        {
                            dataRowLineNumber = Int32.Parse(receiveDataString, System.Globalization.NumberStyles.HexNumber);
                            //Console.WriteLine("Data Row line : {0}", dataRowLineNumber);
                        }
                        else if (dataCounter > 2 && dataCounter < (columnNumber*2+3))
                        {
                            int dataInfo = Int32.Parse(receiveDataString, System.Globalization.NumberStyles.HexNumber);


                            int dataMSB = 0;
                            int dataLSB = 0;

                            if(dataPriority % 2 == 0)
                            {
                                dataMSB = dataInfo * 256;
                            }
                            else if (dataPriority % 2 == 1)
                            {
                                dataLSB = dataInfo;
                            }

                            //Console.WriteLine("Data Info[{0:00}][{1:00}] : {2}", dataColLineNumber, dataRowLineNumber, dataInfo);
                            //dataArrayPressString[dataColLineNumber, dataRowLineNumber] = String.Format("{0:00},{0:00},", dataColLineNumber+1, dataRowLineNumber+1);
                            //dataArrayPressString[dataColLineNumber, dataRowLineNumber] = String.Format("{0:0000}", dataMSB + dataLSB);

                            dataPriority++;
                            if (dataPriority > columnNumber*2-1)
                            {
                                dataPriority = 0;
                            }

                            dataColLineNumber++;
                            if (dataColLineNumber > columnNumber-1)
                            {
                                dataColLineNumber = 0;
                            }
                        }
                        dataCounter++;
                        if (dataCounter > 50)
                        {
                            dataCounter = 0;
                            trigger = false;
                        }
                    }


                    if (receiveDataString == "FD")
                    {
                        dataSaverString[0] = receiveDataString;
                    }

                    if (dataSaverString[0] == "FD" && receiveDataString == "FE")
                    {
                        //Console.WriteLine("Data End");
                    }

                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }
        private void dataLogBox_TextChanged(object sender, EventArgs e)
        {
            dataLogBox.SelectionStart = dataLogBox.TextLength;     //스크롤 자동으로 내린다.
            dataLogBox.ScrollToCaret();
        }
        private void DataReceviedTextBoxLogging(string log)
        {
            log = log.Replace("\r", "").Replace("\n", "");
            dataLogBox.Text += log + Environment.NewLine;
        }


        /* #2 Layout Grid Setting*/
        private void button_layout_Click(object sender, EventArgs e)
        {
            try
            {
                createGridViewForm();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }
        
        private void graphicVIewSize_changed(object sender, EventArgs e)
        {
            try
            {
                if(dataGridView_ScaleControl.ColumnCount != 0)
                {
                    resizeGridViewForm();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }
        }

        private int[,] setLayoutValueInt(int column, int row)
        {
            int[,] dataArray= new int[column, row];
            return dataArray;
        }
        private string[,] setLayoutValueString(int column, int row)
        {
            string[,] dataArray = new string[column, row];
            return dataArray;
        }
        public void createGridViewForm()
        {
            //Grid Editable
            dataGridView_ScaleControl.ReadOnly = true;
            //Grid Count
            dataGridView_ScaleControl.ColumnCount = columnNumber;
            dataGridView_ScaleControl.RowCount = rowNumber;
            //Grid BoxResize
            dataGridView_ScaleControl.RowHeadersWidth = 50;

            //Grid Styles : font
            dataGridView_ScaleControl.Font = new Font("맑은 고딕", 7, FontStyle.Bold);
            //dataGridView_ScaleControl.ColumnHeadersDefaultCellStyle.Font = new Font("맑은 고딕", 8, FontStyle.Bold);
            //dataGridView_ScaleControl.RowHeadersDefaultCellStyle.Font = new Font("맑은 고딕", 8, FontStyle.Bold);
            //dataGridView_ScaleControl.DefaultCellStyle.Font = new Font("맑은 고딕", 8, FontStyle.Bold);

            for (int x = 0; x <= dataGridView_ScaleControl.Columns.Count - 1; x++)
            {
                dataGridView_ScaleControl.Columns[x].HeaderText = String.Format("{0:00}", x + 1);
                dataGridView_ScaleControl.Columns[x].Width = (dataGridView_ScaleControl.Width / (dataGridView_ScaleControl.Columns.Count + 2));
            }
            for (int y = 0; y <= dataGridView_ScaleControl.Rows.Count - 1; y++)
            {
                dataGridView_ScaleControl.Rows[y].HeaderCell.Value = String.Format("{0:00}", y + 1);
                dataGridView_ScaleControl.Rows[y].Height = (dataGridView_ScaleControl.Height / (dataGridView_ScaleControl.Rows.Count + 2));
            }
        }

        public void resizeGridViewForm()
        {
            for (int x = 0; x <= dataGridView_ScaleControl.Columns.Count - 1; x++)
            {
                dataGridView_ScaleControl.Columns[x].Width = (dataGridView_ScaleControl.Width / (dataGridView_ScaleControl.Columns.Count + 2));
            }
            for (int y = 0; y <= dataGridView_ScaleControl.Rows.Count - 1; y++)
            {
                dataGridView_ScaleControl.Rows[y].Height = (dataGridView_ScaleControl.Height / (dataGridView_ScaleControl.Rows.Count + 2));
            }
        }

        /* #Extra Color Array List */
        List<Color> colorStepArrayBackGround = new List<Color>();
        List<Color> colorStepArrayFont = new List<Color>();
        List<Color> colorStepArray0_BTC = GetGradientColors(Color.Blue, Color.Cyan, 1024);
        List<Color> colorStepArray1_CTG = GetGradientColors(Color.Cyan, Color.Green, 1024);
        List<Color> colorStepArray2_GTY = GetGradientColors(Color.Green, Color.Yellow, 1024);
        List<Color> colorStepArray3_YTR = GetGradientColors(Color.Yellow, Color.Red, 1024);
        List<Color> colorStepArray_WTW = GetGradientColors(Color.White, Color.White, 512);
        List<Color> colorStepArray_WTB = GetGradientColors(Color.White, Color.Black, 512);
        List<Color> colorStepArray_BTB = GetGradientColors(Color.Black, Color.Black, 512);
        List<Color> colorStepArray_BTW = GetGradientColors(Color.Black, Color.White, 512);
        public void color_initialize()
        {
            /* Background */
            colorStepArrayBackGround.AddRange(colorStepArray0_BTC);
            colorStepArrayBackGround.AddRange(colorStepArray1_CTG);
            colorStepArrayBackGround.AddRange(colorStepArray2_GTY);
            colorStepArrayBackGround.AddRange(colorStepArray3_YTR);
            /* Font */
            colorStepArrayFont.AddRange(colorStepArray_WTB);//0000~0511
            colorStepArrayFont.AddRange(colorStepArray_BTB);//0512~1023
            colorStepArrayFont.AddRange(colorStepArray_BTW);//1024~1535
            colorStepArrayFont.AddRange(colorStepArray_WTW);//1536~2047
            colorStepArrayFont.AddRange(colorStepArray_WTW);//2048~2559
            colorStepArrayFont.AddRange(colorStepArray_WTB);//2560~3071
            colorStepArrayFont.AddRange(colorStepArray_BTW);//3072~3583
            colorStepArrayFont.AddRange(colorStepArray_WTW);//3584~4095
        }
        public static List<Color> GetGradientColors(Color start, Color end, int steps)
        {
            return GetGradientColors(start, end, steps, 0, steps);
        }
        public static List<Color> GetGradientColors(Color start, Color end, int steps, int firstStep, int lastStep)
        {
            var colorList = new List<Color>();
            /*
            if (steps <= 0 || firstStep < 0 || lastStep > 256)
            {
                return colorList;
            }


            double aStep = (end.A - start.A) / (steps - 1);
            double rStep = (end.R - start.R) / (steps - 1);
            double gStep = (end.G - start.G) / (steps - 1);
            double bStep = (end.B - start.B) / (steps - 1);
            */

            double diffA = end.A - start.A;
            double diffR = end.R - start.R;
            double diffG = end.G - start.G;
            double diffB = end.B - start.B;


            var stepA = diffA / (steps - 1);
            var stepR = diffR / (steps - 1);
            var stepG = diffG / (steps - 1);
            var stepB = diffB / (steps - 1);

            for (int i = firstStep; i < lastStep; i++)
            {
                /*
                var a = start.A + (int)(aStep * i);
                var r = start.R + (int)(rStep * i);
                var g = start.G + (int)(gStep * i);
                var b = start.B + (int)(bStep * i);

                colorList.Add(Color.FromArgb(a, r, g, b));
                */

                int c(int color, double step)
                {
                    return (int)Math.Round(color + step * i);
                }
                colorList.Add(
                    Color.FromArgb(
                        c(start.A, stepA),
                        c(start.R, stepR),
                        c(start.G, stepG),
                        c(start.B, stepB))
                );

        }
            return colorList;
        }

        /* #3-0 Record Setting : Value */
        delegate void TimerEventFiredDelegate();
        System.Timers.Timer timer = null;
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        TimeSpan timSpan = new TimeSpan(0, 0, 0, 0);
        int click_loop = 0;
        int dataIndex = 0;
        int columnCounter = 0;
        int rowCounter = 0;

        /* #3-1 Record Setting : Timer Initialization */
        public void timer_initialize()
        {
            textBox_timer.Text = String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);
            timer = new System.Timers.Timer();
            timer.Interval = 1;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        /* #3-2 Record Setting : Timer Elapsed Event And Worker Setting  */
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Timer_Worker));
        }
        private void Timer_Worker()
        {
            /* Time Span Log */
            timSpan = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                        timSpan.Hours, timSpan.Minutes, timSpan.Seconds,
                        timSpan.Milliseconds / 10);
            textBox_timer.Text = elapsedTime;

            /* Data - Array Example Load */ /**/
            for (rowCounter = 0; rowCounter < rowNumber; rowCounter++)
            {
                for(columnCounter = 0; columnCounter < columnNumber; columnCounter++)
                {
                    dataArrayValue[columnCounter, rowCounter] = dataIndex;
                }
            }
            
            /* Data - Array View */ /**/
            for (rowCounter = 0; rowCounter < rowNumber; rowCounter++)
            {
                for (columnCounter = 0; columnCounter < columnNumber; columnCounter++)
                {
                    /* SET DATA of CELL */
                    dataGridView_ScaleControl[columnCounter, rowCounter].Value = String.Format("{0:0000}", dataArrayValue[columnCounter, rowCounter]);
                    //dataGridView_ScaleControl[columnCounter, rowCounter].Value = dataArrayPressString[columnCounter, rowCounter];
                   
                    /* SET BACKGROUND COLOR */
                    dataGridView_ScaleControl[columnCounter, rowCounter].Style.BackColor = colorStepArrayBackGround[dataIndex];
                    dataGridView_ScaleControl[columnCounter, rowCounter].Style.ForeColor = colorStepArrayFont[dataIndex];
                    /* DATA LOGGIN */
                    //DataReceviedTextBoxLogging(dataArrayString[columnCounter, rowCounter]);


                }
            }

            /* Data - Array Example DataIndex */ /**/
            dataIndex++;

            if (dataIndex > 4095)
            {
                dataIndex = 0;
            }
           
        }
        private void button_record_play_Click(object sender, EventArgs e)
        {
            try
            {
                if (click_loop == 0 && dataGridView_ScaleControl.Columns.Count != 0)
                {
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
                }
                else if (click_loop == 1 && dataGridView_ScaleControl.Columns.Count != 0)
                {
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error" ,MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        MessageBox.Show("저장을 완료하였습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void button_record_reset_Click(object sender, EventArgs e)
        {
            try
            {
                if (timer.Enabled == false)
                {
                    if (MessageBox.Show("데이터를 초기화 하시겠습니까?", "데이터 초기화", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        /*Data Reset*/
                        dataGridView_ScaleControl.Columns.Clear();
                        /*TImer Reset*/
                        timer.Close();
                        stopWatch.Reset();
                        timSpan = new TimeSpan(0, 0, 0, 0);
                        dataIndex = 0;
                        textBox_timer.Text = String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);

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

    }
}
