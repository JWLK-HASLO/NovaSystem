using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NovaSystem
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            FormConnectionLoad();
        }

        
        //Form Closing Event Handler
        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void tabMainControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMainControl.SelectedTab == tabPage1_connection)
            {
                //MessageBox.Show("Connection PAGE");
                dataViewBoxWrap.Visible = true;
            }
            else 
            {
                //MessageBox.Show("other PAGE");
                dataViewBoxWrap.Visible = false;
            }
        }

        
    }
}
