using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echo_system
{
    public partial class Overlay : Form
    {
        private bool _isdrawing = false;
        private bool _isDrawingActive = false;
        private Point _lastPoint = Point.Empty;
        private List<List<Point>> _points = new List<List<Point>>();
        private List<Point> _currpoints = new List<Point>();
        int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
        int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = -20;
        private const uint LWA_COLORKEY = 0x1;
        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int LWA_ALPHA = 0x2;

        private Form leftchildForm;


        public Overlay()
        {
            InitializeComponent();
        }

        private void Overlay_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.DoubleBuffered = true;

            // Make the form layered and transparent
            int style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            SetLayeredWindowAttributes(this.Handle, 0, 0, LWA_ALPHA);

            CreateLeftBar();
        }

        private void CreateLeftBar()
        {
            Size barsize = new Size(50, 200);
            leftchildForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Size = barsize,
                BackColor = Color.White,
                TopMost = true,
                ShowInTaskbar = false,
            };

            // Lock the form size to prevent resizing
            leftchildForm.MaximumSize = barsize;
            leftchildForm.MinimumSize = barsize;

            int style = GetWindowLong(leftchildForm.Handle, GWL_EXSTYLE);
            SetWindowLong(leftchildForm.Handle, GWL_EXSTYLE, style | WS_EX_TOOLWINDOW);

            leftchildForm.MinimumSize = barsize;
            leftchildForm.AutoSize = false;
            leftchildForm.Location = new Point(0, (int)(screenHeight * 0.5));


            leftchildForm.Controls.Add(CreateButton("", "", "Drawing", 15, 10, 20, 20, DrawMode));
            leftchildForm.Controls.Add(CreateButton("", "Button 2", "hhhh", 15, 40, 20, 20, DrawMode));
            leftchildForm.Controls.Add(CreateButton("", "Button 3", "", 15, 70, 20, 20, DrawMode));
            leftchildForm.Controls.Add(CreateButton("", "Button 4", "", 15, 100, 20, 20, DrawMode));


            leftchildForm.Show();
        }

        private Button CreateButton(string imgpath, string text, string tooltiptext, int x, int y, int width, int height, EventHandler handler)
        {
            Image yourIcon = null;

            if (imgpath != "")
            {
                yourIcon = Image.FromFile(imgpath);
            }
            

            Button button = new Button
            {
                Text = text,
                Image = yourIcon,
                Size = new Size(width, height),
                Location = new Point(x, y),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                BackgroundImageLayout = ImageLayout.Zoom,
            };

            
            button.FlatAppearance.BorderSize = 0;
            button.Click += (sender, args) => handler(sender, args);
            ToolTip tp = new ToolTip
            {
                AutoPopDelay = 0,
                InitialDelay = 0,
                ReshowDelay = 0,
            };

            tp.SetToolTip(button, tooltiptext);
            tp.ShowAlways = true;
            return button;
        }

        #region Drawmode

        #region Variables
        private Color choice = Color.Red;
        private float thickness = 3;
        private Form ChoicePanel;
        private TrackBar thicknessSlider;
        private Button colorButton;
        #endregion

        #region Drawing
        private void DrawMode(object sender, EventArgs e)
        {
            _isdrawing = !_isdrawing;

            int style = GetWindowLong(this.Handle, GWL_EXSTYLE);

            if (_isdrawing)
            {

                SetWindowLong(this.Handle, GWL_EXSTYLE, style & ~WS_EX_TRANSPARENT);
                SetLayeredWindowAttributes(this.Handle, 0, 150, LWA_ALPHA);


                this.MouseDown += Form_MouseDown;
                this.MouseMove += Form_MouseMove;
                this.MouseUp += Form_MouseUp;

                showpensettings();
                ChoicePanel.BringToFront();


            }
            else
            {

                SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
                SetLayeredWindowAttributes(this.Handle, 0, 0, LWA_ALPHA);


                this.MouseDown -= Form_MouseDown;
                this.MouseMove -= Form_MouseMove;
                this.MouseUp -= Form_MouseUp;
                _points.Clear();
                
                closepensettings(); 

                this.Invalidate();
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (_isdrawing)
            {
                _isDrawingActive = true;
                _lastPoint = e.Location;


                _currpoints.Clear();
                _currpoints.Add(e.Location);
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isdrawing && _isDrawingActive)
            {

                _currpoints.Add(e.Location);
                this.Invalidate();
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isdrawing)
            {

                _isDrawingActive = false;


                if (_currpoints.Count > 1)
                {
                    _points.Add(new List<Point>(_currpoints));
                }


                _currpoints.Clear();
                _lastPoint = Point.Empty;


                this.Invalidate();
            }

            leftchildForm.BringToFront();
            ChoicePanel.BringToFront();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
             
            using (Pen pen = new Pen(choice, thickness))
            {

                foreach (var drawing in _points)
                {
                    for (int i = 1; i < drawing.Count; i++)
                    {
                        e.Graphics.DrawLine(pen, drawing[i - 1], drawing[i]);
                    }
                }


                if (_isdrawing && _isDrawingActive)
                {
                    for (int i = 1; i < _currpoints.Count; i++)
                    {
                        e.Graphics.DrawLine(pen, _currpoints[i - 1], _currpoints[i]);
                    }
                }
            }
        }
        #endregion

        #region settings

        private bool isDragging = false;
        private Point dragStartPoint = Point.Empty;
        private void showpensettings()
        {
            if (ChoicePanel == null)
            {
                ChoicePanel = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.Manual,
                    Size = new Size(200, 200),
                    BackColor = Color.White,
                    TopMost = true,
                    ShowInTaskbar = false
                };

                ChoicePanel.MouseDown += ChoicePanel_MouseDown;
                ChoicePanel.MouseMove += ChoicePanel_MouseMove;
                ChoicePanel.MouseUp += ChoicePanel_MouseUp;
            }

            ChoicePanel.Show();
        }

        private void closepensettings()
        {
            if (ChoicePanel != null)
            {
                ChoicePanel.Hide();
            }
        }


        private void ChoicePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = new Point(e.X, e.Y);
            }
        }

        private void ChoicePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                ChoicePanel.Location = new Point(
                    ChoicePanel.Left + e.X - dragStartPoint.X,
                    ChoicePanel.Top + e.Y - dragStartPoint.Y);
            }
        }

        private void ChoicePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }
        #endregion

        #endregion
    }
}