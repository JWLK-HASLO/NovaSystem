using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NovaSystem 
{ 
    public partial class MainForm : Form
    {
        /* #1-1 Data Receive*/
        StreamWriter streamWriter;
        FileStream fileStream;
        int stringDataCounter = 0;
        int stringDataCounterSaver = 0;
        bool Header_Flag = false;
        bool Press_Flag = false;
        bool Temp_Flag = false;

        int bufferCounter = 0;
        static int bufferDataLength = 51;

        string bufferSUMLine = null;
        string[] BufferSUM = new string[bufferDataLength];
        // BufferSUM[0] = Press(50) or Temp(54)
        // BufferSUM[1] = Colum
        // BufferSUM[2] = Row

        // N = 0 ~ 23
        // BufferSUM[2N+3] = HSB // BufferSUM[2N+4] = LSB

        // BufferSUM[3] = HSB // BufferSUM[4] = LSB
        // BufferSUM[5] = HSB // BufferSUM[6] = LSB
        // BufferSUM[7] = HSB // BufferSUM[8] = LSB
        // BufferSUM[9] = HSB // BufferSUM[10] = LSB
        // BufferSUM[11] = HSB // BufferSUM[12] = LSB
        // BufferSUM[13] = HSB // BufferSUM[14] = LSB
        // BufferSUM[15] = HSB // BufferSUM[16] = LSB
        // BufferSUM[17] = HSB // BufferSUM[18] = LSB
        // --------------------------------------------- //

        // BufferSUM[47] = HSB // BufferSUM[48] = LSB
        // BufferSUM[49] = HSB // BufferSUM[50] = LSB

        private int dataIntConverstion(int GET_DATA)
        {
            if (GET_DATA > 0 && GET_DATA < 10)
            {
                GET_DATA = 1;
            }
            else if (GET_DATA >= 10 & GET_DATA < 50)
            {
                GET_DATA += 0;
            }
            else if (GET_DATA >= 50 & GET_DATA < 500)
            {
                GET_DATA += 10;
            }
            else if (GET_DATA >= 500 & GET_DATA < 1000)
            {
                GET_DATA += 40;
            }
            else if (GET_DATA >= 1000 & GET_DATA < 1500)
            {
                GET_DATA += 20;
            }
            else if (GET_DATA >= 1500 & GET_DATA < 2500)
            {
                GET_DATA += 10;
            }
            else if (GET_DATA >= 2500 & GET_DATA < 3500)
            {
                GET_DATA += 20;
            }
            else if (GET_DATA >= 3500 & GET_DATA < 3900)
            {
                GET_DATA += 17;
            }
            else if (GET_DATA >= 3900 & GET_DATA < 4000)
            {
                GET_DATA += 0;
            }
            else if (GET_DATA >= 4020)
            {
                GET_DATA -= 5;
            }


            return GET_DATA;
        }
        private void AddGridTable(string bufferLine)
        {
            string ind_press = bufferLine.Substring(2 * 0, 2 * 1);
            string ind_row = bufferLine.Substring(2 * 2, 2 * 1);
            int row = Int32.Parse(ind_row, System.Globalization.NumberStyles.HexNumber);

            for(int n = 0; n < 24; n++)
            {
                //string HSB = bufferLine.Substring(2 * 3, 2 * 1);
                //string LSB = bufferLine.Substring(2 * 4, 2 * 1);
                //string HSB = bufferLine.Substring(2 * 5, 2 * 1);
                //string LSB = bufferLine.Substring(2 * 6, 2 * 1);

                string String_HSB = bufferLine.Substring(2 * (2 * n + 3), 2);
                string String_LSB = bufferLine.Substring(2 * (2 * n + 4), 2);
                int INT_HSB = Int32.Parse(String_HSB, System.Globalization.NumberStyles.HexNumber);
                int INT_LSB = Int32.Parse(String_LSB, System.Globalization.NumberStyles.HexNumber);
                int SUM_DATA = INT_HSB * 256 + INT_LSB;

                dataArrayPressString[n, row] = String.Format("{0:0000}", dataIntConverstion(SUM_DATA));
                //dataArrayPressString[n, row] = String.Format("{0:0000}", SUM_DATA);
                //Console.WriteLine("AddGridTable {0}", SUM_DATA);
            }
            
        }

        private void ConvertByteToBufferSum(string stringData) 
        {
            try
            {
                if (stringData.Equals("FC"))
                {
                    stringDataCounterSaver = stringDataCounter;
                }

                if ((stringDataCounterSaver + 1) == stringDataCounter && stringData.Equals("FD"))
                {
                    Header_Flag = true;
                }

                if ((stringDataCounterSaver + 2) == stringDataCounter && Header_Flag && stringData.Equals("50"))
                {
                    Press_Flag = true;
                    bufferCounter = 0;
                }

                if (Press_Flag && bufferCounter < bufferDataLength)
                {
                    //Console.WriteLine("HEX Data: {0}  ", receiveDataString);

                    BufferSUM[bufferCounter] = receiveDataString;
                    bufferSUMLine += BufferSUM[bufferCounter];

                    bufferCounter++;

                    if (bufferCounter == 51)
                    {
                        //Console.WriteLine("bufferSUMLine: {0}  ", bufferSUMLine);
                        AddGridTable(bufferSUMLine);
                        /*Data Save*/
                       
                        streamWriter.WriteLine(String.Format("{0}/{1}", textBox_timer.Text, bufferSUMLine));

                        bufferSUMLine = null;

                    }

                }
                else
                {
                    Press_Flag = false;
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(exception.Message);
            }

            stringDataCounter++;

        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort portSender = (SerialPort)sender;

                /*Rade Data Byte*/
                byte[] byteBuffer = new byte[portSender.BytesToRead];
                portSender.Read(byteBuffer, 0, byteBuffer.Length);
                //PortBuffer.AddRange(byteBuffer);
                //Console.WriteLine(String.Join(",", byteBuffer));

                if (click_loop == 1 && dataGridView_ScaleControl.Columns.Count != 0)
                {
                    if(byteBuffer.Length != 0)
                    {
                        //Console.WriteLine("byteBuffer Lenght : {0}", byteBuffer.Length);
                        for (int iTemp = 0; iTemp < byteBuffer.Length; iTemp++)
                        {
                            receiveDataString = byteBuffer[iTemp].ToString("X2");
                            ConvertByteToBufferSum(receiveDataString);
                            //Console.WriteLine("HEX: {0}  ", receiveDataString);

                        }
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



        /* #3-0 Record Setting : Value */
        delegate void TimerEventFiredDelegate();
        System.Timers.Timer timer = null;
        System.Timers.Timer timerReadData = null;

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        TimeSpan timSpan = new TimeSpan(0, 0, 0, 0);

        String dataValueString = "";
        int click_loop = 0;
        int columnCounter = 0;
        int rowCounter = 0;

        int dataIndex = 0;
        int raiseCounter = 0;
        private object system;
        private object int32;

        /* #3-1 Record Setting : Timer Initialization */
        public void timer_initialize()
        {
            textBox_timer.Text = String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);
            textBox_timer_import.Text = String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0);
            timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }
       
        public void timer_read_initialize()
        {
            timerReadData = new System.Timers.Timer();
            timerReadData.Interval = 1;
            timerReadData.Elapsed += new System.Timers.ElapsedEventHandler(timer_read_Elapsed);
        }

        /* #3-2 Record Setting : Timer Elapsed Event And Worker Setting  */
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Timer_Worker));
        }
        private void timer_read_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(new TimerEventFiredDelegate(Timer_Read_Worker));
        }

        private void Timer_Read_Worker()
        {
            if (streamReader.EndOfStream)
            {
                timerReadData.Stop();
                MessageBox.Show("모든 기록을 확인하였습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string readSUMLine = streamReader.ReadLine();
                string[] splitReadLine = readSUMLine.Split('/');
                //Console.WriteLine("Data Test {0} : {1}", splitReadLine[0], splitReadLine[1]);
                AddGridTable(splitReadLine[1]);
                textBox_timer_import.Text = splitReadLine[0];
            }
        }
        private void Timer_Worker()
        {
            /* Time Span Log */
            timSpan = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                        timSpan.Hours, timSpan.Minutes, timSpan.Seconds,
                        timSpan.Milliseconds / 10);
            textBox_timer.Text = elapsedTime;
            setGridClickPosition((selectedColCount+1).ToString(), (selectedRowCount+1).ToString()); ;

            setGridClickData(
                dataArrayPressString[selectedColCount, selectedRowCount], 
                pressExchange(dataArrayPressString[selectedColCount, selectedRowCount]),
                registerExchange(dataArrayPressString[selectedColCount, selectedRowCount])
                );

            ///* Data - Array Example Load */ /**/
            //for (int i = 0; i < 14; i++, raiseCounter++)
            //{
            //    for (int j = 0; j < 24; j++)
            //    {
            //        dataArrayValue[j, i] = raiseCounter;
            //    }
            //}


            /* Data - Array View */ /**/
            for (rowCounter = 0; rowCounter < rowNumber; rowCounter++)
            {
                for (columnCounter = 0; columnCounter < columnNumber; columnCounter++)
                {
                    /* SET DATA of CELL */
                    dataValueString = dataArrayPressString[columnCounter, rowCounter];
                    
                    if(rbtn_press.Checked == true)
                    {
                        if(dataValueString != null && dataValueString != "0000" )
                        {
                            dataGridView_ScaleControl[columnCounter, rowCounter].Value = pressExchange(dataValueString);
                        }
                        else
                        {
                            dataGridView_ScaleControl[columnCounter, rowCounter].Value = "";
                        }
                    }
                    else if(rbtn_register.Checked == true)
                    {
                        if (dataValueString != null && dataValueString != "0000")
                        {
                            dataGridView_ScaleControl[columnCounter, rowCounter].Value = registerExchange(dataValueString);
                        }
                        else
                        {
                            dataGridView_ScaleControl[columnCounter, rowCounter].Value = "";
                        }
                    }
                    else if(rbtn_adc.Checked == true)
                    {
                        dataGridView_ScaleControl[columnCounter, rowCounter].Value = dataValueString;
                    }
                    else if(rbtn_text_none.Checked == true)
                    {
                        dataGridView_ScaleControl[columnCounter, rowCounter].Value = "";
                    }
                    

                    /* SET BACKGROUND COLOR */
                    if(rbtn_color_on.Checked == true)
                    {
                        dataGridView_ScaleControl[columnCounter, rowCounter].Style.BackColor = colorStepArrayBackGround[Int32.Parse(dataValueString == null ? "0" : dataValueString)];
                        dataGridView_ScaleControl[columnCounter, rowCounter].Style.ForeColor = colorStepArrayFont[Int32.Parse(dataValueString == null ? "0" : dataValueString)];
                    }
                    else if(rbtn_color_off.Checked == true)
                    {
                        dataGridView_ScaleControl[columnCounter, rowCounter].Style.BackColor = System.Drawing.Color.White;
                        dataGridView_ScaleControl[columnCounter, rowCounter].Style.ForeColor = System.Drawing.Color.Black;
                    }

                    //dataGridView_ScaleControl[columnCounter, rowCounter].Value = String.Format("{0:0000}", dataArrayValue[columnCounter, rowCounter]);
                    //dataGridView_ScaleControl[columnCounter, rowCounter].Style.BackColor = colorStepArrayBackGround[dataIndex];
                    //dataGridView_ScaleControl[columnCounter, rowCounter].Style.ForeColor = colorStepArrayFont[dataIndex];
                    /* DATA LOGGIN */
                    //DataReceviedTextBoxLogging(dataArrayString[columnCounter, rowCounter]);


                }
            }

            ///* Data - Array Example DataIndex *//**/
            //dataIndex++;

            //if (dataIndex > 4095)
            //{
            //    dataIndex = 0;
            //}

        }

    }
}
