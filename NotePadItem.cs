using Echo_system.Utility_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echo_system
{
    public partial class NotePadItem : UserControl
    {
        private bool _selected = false;
        private Form Content;
        public NotePadItem(DateTime ModifiedDate, string Title)
        {
            InitializeComponent();
            textBox1.Text = Title;
            textBox2.Text = ModifiedDate.ToString();

            checkBox1.Hide();
            
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                checkBox1.Checked = true;
                _selected = true;
                this.BackColor = Color.Blue;

            }
            else
            {
                checkBox1.Checked = false;
                _selected = false;
                this.BackColor = Color.DeepSkyBlue;
            }
        }

        public void ShowContent()
        {

        }
    }
}
