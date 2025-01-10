using Echo_system.AI_System;
using Echo_system.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
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

        private AITextChat textForm;
        private CharacterForm videoForm;

    


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


            int style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            SetLayeredWindowAttributes(this.Handle, 0, 0, LWA_ALPHA);

            CreateLeftBar();
            CreateBottomBar();
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
                Location = new Point(0, (int)(screenHeight * 0.5))
            };

            leftchildForm.MaximumSize = barsize;
            leftchildForm.MinimumSize = barsize;

            int style = GetWindowLong(leftchildForm.Handle, GWL_EXSTYLE);
            SetWindowLong(leftchildForm.Handle, GWL_EXSTYLE, style | WS_EX_TOOLWINDOW);

            leftchildForm.MinimumSize = barsize;
            leftchildForm.AutoSize = false;



            leftchildForm.Controls.Add(CreateButton(Resources.Icons.pen_icon, "", "Drawing Board", 15, 10, 20, 20, DrawMode));
            leftchildForm.Controls.Add(CreateButton(Resources.Icons.calander_icon, "", "Scheduler", 15, 50, 20, 20, Scheduler));



            leftchildForm.Show();
        }



        public void CreateBottomBar()
        {
            videoForm = new CharacterForm("test", new Size(screenWidth, screenHeight), new Size(200, 250), new Size(screenWidth, screenHeight));
            videoForm.Show();
            // Create a form for the text box
            Size textFormSize = new Size(400, 60);
            textForm = new AITextChat(textFormSize, new Size(screenWidth, screenHeight), videoForm);
            textForm.Show();
            
        }



        private Button CreateButton(Image imgpath, string text, string tooltiptext, int x, int y, int width, int height, EventHandler handler)
        {
            Image yourIcon = imgpath;
            

            if (yourIcon != null)
            {
                yourIcon = ResizeImage(yourIcon, width, height);
            }


            Button button = new Button
            {
                Text = text,
                Image = yourIcon,
                Size = new Size(width, height),
                Location = new Point(x, y),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                BackgroundImageLayout = ImageLayout.Stretch,
                ImageAlign = ContentAlignment.MiddleCenter
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

        private Image ResizeImage(Image image, int width, int height)
        {

            float aspectRatio = (float)image.Width / image.Height;
            int newWidth = width;
            int newHeight = height;

            if (width / aspectRatio <= height)
            {
                newHeight = (int)(width / aspectRatio);
            }
            else
            {
                newWidth = (int)(height * aspectRatio);
            }

            return new Bitmap(image, newWidth, newHeight);
        }

        #region Drawmode

        #region Variables
        private bool _isdrawing = false;
        private bool _isDrawingActive = false;
        private Point _lastPoint = Point.Empty;

        private Dictionary<Tuple<Color, float>, List<List<Point>>> _points = 
            new Dictionary<Tuple<Color, float>, List<List<Point>>>();

        private List<Point> _currpoints = new List<Point>();


        private Color choice = Color.Red;
        private float thickness = 3;
        private Form ChoicePanel;
        private TrackBar thicknessSlider;
        private Button colorButton;
        private Button clearButton;
        private ColorDialog colorDialog;

        ToolTip ctp = new ToolTip
        {
            AutoPopDelay = 0,
            InitialDelay = 0,
            ReshowDelay = 0,
        };
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
                
                closepensettings(); 

                if (Properties.Settings.Default.ClearDrawingOnClose)
                {
                    _points.Clear();
                }

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
                    var key = Tuple.Create(choice, thickness);


                    if (!_points.ContainsKey(key))
                    {
                        _points[key] = new List<List<Point>>();
                    }


                    _points[key].Add(new List<Point>(_currpoints)); 

                    _currpoints.Clear();
                }

                _lastPoint = Point.Empty;


                this.Invalidate();
            }

            leftchildForm.BringToFront();
            ChoicePanel.BringToFront();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);


            foreach (var entry in _points)
            {

                var color = entry.Key.Item1; 
                var thickness = entry.Key.Item2; 


                using (Pen pen = new Pen(color, thickness))
                {

                    var strokes = entry.Value;


                    foreach (var points in strokes)
                    {
                        for (int i = 1; i < points.Count; i++)
                        {
                            e.Graphics.DrawLine(pen, points[i - 1], points[i]);
                        }
                    }
                }
            }


            if (_isdrawing && _isDrawingActive)
            {
                using (Pen pen = new Pen(choice, thickness))
                {
                    for (int i = 1; i < _currpoints.Count; i++)
                    {
                        e.Graphics.DrawLine(pen, _currpoints[i - 1], _currpoints[i]);
                    }
                }
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _points.Clear();
            _currpoints.Clear();

            this.Invalidate();
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
                    Size = new Size(250, 50),
                    Location = new Point(100, 100),
                    BackColor = Color.White,
                    TopMost = true,
                    ShowInTaskbar = false
                };

                colorButton = new Button
                {
                    Location = new Point(20, 10),
                    Size = new Size(30, 30),
                    BackColor = choice,
                };
                colorButton.Click += ColorButton_Click;


                thicknessSlider = new TrackBar
                {
                    Minimum = 1,
                    Maximum = 10,
                    Value = (int)thickness,
                    TickStyle = TickStyle.Both,
                    Location = new Point(60, 5),
                    Size = new Size(100, 30)
                };
                thicknessSlider.ValueChanged += ThicknessSlider_ValueChanged;


                Image clearbtnimg = Resources.Icons.clear_icon;
                clearbtnimg = ResizeImage(clearbtnimg, 30, 30);

                clearButton = new Button
                {
                    Location = new Point(180, 10),
                    Size = new Size(30, 30),
                    BackColor = Color.Transparent,
                    Image = clearbtnimg,

                    BackgroundImageLayout = ImageLayout.Stretch,
                    ImageAlign = ContentAlignment.MiddleCenter

                };

                clearButton.Click += clearButton_Click;

                ctp.SetToolTip(clearButton, "Clear Board");
                ctp.SetToolTip(colorButton, "Select Color");
                ctp.SetToolTip(thicknessSlider, "Set Brush Thickness");

                


                colorDialog = new ColorDialog();


                ChoicePanel.Controls.Add(colorButton);
                ChoicePanel.Controls.Add(thicknessSlider);
                ChoicePanel.Controls.Add(clearButton);

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

        private void ColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                choice = colorDialog.Color;
                colorButton.BackColor = choice;
            }
        }

        private void ThicknessSlider_ValueChanged(object sender, EventArgs e)
        {
            thickness = thicknessSlider.Value; 
        }


        #endregion

        #endregion

        #region Scheduler
        private Form Schedule;
        private void Scheduler(object sender, EventArgs e)
        {

            if (Schedule == null || Schedule.IsDisposed)
            {
                Schedule = new Calander();
            }


            if (!Schedule.Visible)
            {
                Schedule.Show();
                Schedule.BringToFront();
            }
            else
            {
                Schedule.Hide();
            }
        }
        #endregion


    }
}