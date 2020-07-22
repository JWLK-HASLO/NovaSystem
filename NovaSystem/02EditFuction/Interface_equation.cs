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
        public string registerExchange(string dataValueString)
        {
            string result = null;
            double dataSaver = 0.0;
            double dataValueInt = (dataValueString == null ? 0 : (double.Parse(dataValueString)));

            //0.00000000013370696448x4 - 0.00000179629337971232x3 + 0.01125961378939790000x2 - 49.73483418142400000000x + 99840.25435951460000000000

            dataSaver = (0.00000000013370696448 * Math.Pow(dataValueInt, 4)) - (0.00000179629337971232 * Math.Pow(dataValueInt, 3)) + (0.01125961378939790000 * Math.Pow(dataValueInt, 2)) - 49.73483418142400000000 * dataValueInt + 99840.25435951460000000000;

            result = Math.Round(dataSaver).ToString();

            return result;
        }

        public string pressExchange(string dataValueString)
        {
            string result = null;
            double dataSaver = 0.0;
            double value = (dataValueString == null ? 0 : (double.Parse(dataValueString)));

            /*Register to Press Equation*/
            dataSaver = (0.00000000013370696448 * Math.Pow(value, 4)) - (0.00000179629337971232 * Math.Pow(value, 3)) + (0.01125961378939790000 * Math.Pow(value, 2)) - 49.73483418142400000000 * value + 99840.25435951460000000000;

            result = Math.Round(dataSaver).ToString();

            return result;
        }

    }
}
