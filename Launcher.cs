using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echo_system
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private void launcher_Load(object sender, EventArgs e)
        {
        }

        private Overlay _overlay;

        private void launchbtn_Click(object sender, EventArgs e)
        {
            if (_overlay == null || _overlay.IsDisposed)
            {
                _overlay = new Overlay();
                _overlay.FormClosed += (s, args) =>
                {
                    _overlay = null;
                    launchbtn.Enabled = true;
                    launchbtn.Text = "Launch!";
                };

                _overlay.Show();

                launchbtn.Text = "Running!";
                launchbtn.Enabled = false;

                this.WindowState = FormWindowState.Minimized;
            }
        }

    }
}
