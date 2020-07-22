using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NovaSystem
{
    public partial class MainForm : Form
    {
        public void gridClickEvent_initialize()
        {
            selectedTextViewMode();
        }

        public void selectedTextViewMode()
        {
            /**
             * Default Radio Button Setting : TEXT VIEW
             */
            rbtn_press.Checked = true;
            //rbtn_adc.Checked = true;
            //rbtn_register.Checked = true;
            //rbtn_text_none.Checked = true;
            /**
             * Default Radio Button Setting : COLOR VIEW
             */
            rbtn_color_on.Checked = true;
            //rbtn_color_off.Checked = true;

        }

        int selectedColCount;
        int selectedRowCount;
        private void gridSelectionChanged(object sender, EventArgs e)
        {
            selectedColCount = dataGridView_ScaleControl.CurrentCell.ColumnIndex;
            selectedRowCount = dataGridView_ScaleControl.CurrentCell.RowIndex;
            String selectedData = dataArrayPressString[selectedColCount, selectedRowCount] == null ? "0" : dataArrayPressString[selectedColCount, selectedRowCount];
            Console.WriteLine("gridSelectionChanged Col : {0} / Row : {1} = Data : {2} ", selectedColCount, selectedRowCount, selectedData);
        }
        public void setGridClickPosition(string columnString, string rowString )
        {
            position_column.Text = columnString;
            position_row.Text = rowString;
        }

        public void setGridClickData(string adcText, string pressText, string registerText)
        {
            selected_adc.Text = adcText;
            selected_press.Text = pressText;
            selected_register.Text = registerText;
        }


    }
}
