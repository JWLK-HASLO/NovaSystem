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
        private void button_newRecord_Click(object sender, EventArgs e)
        {

            Form childForm = new LayoutForm();
            childForm.MdiParent = this;
            childForm.Text = "New Data " + childFormNumber++;
            childForm.Show();


        }

        private void button_loadExistRecord_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "CSV 파일 (*.csv)|*.csv|모든 파일 (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
                Form childForm = new LayoutForm();
                childForm.MdiParent = this;
                childForm.Text = FileName;
                childForm.Show();
            }
        }
    }
}
