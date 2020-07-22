using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NovaSystem
{
    public partial class MainForm : Form
    {
        /* #2 Layout Grid Setting*/
        private void button_layout_Click(object sender, EventArgs e)
        {
            try
            {
                createGridViewForm();
                setGridClickPosition("Set","Set");
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
                if (dataGridView_ScaleControl.ColumnCount != 0)
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
            int[,] dataArray = new int[column, row];
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
            Console.WriteLine("colorStepArray Length" + colorStepArrayBackGround.Count);
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

    }
}
