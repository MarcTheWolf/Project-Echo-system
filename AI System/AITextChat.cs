using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echo_system.AI_System
{
    public partial class AITextChat : Form
    {
        private TextBox textbox;
        private Button submitbtn;
        private CharacterForm _characterform;
        private Size _textFormSize;
        private Form triggerForm;

        private string _userInput;
        private Size replyFormSize = new Size(300,150);

        public AITextChat(Size textFormSize, Size Screensize, CharacterForm characterform)
        {
            InitializeComponent();

            _textFormSize = textFormSize;
            _characterform = characterform;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Size = textFormSize;
            MinimumSize = textFormSize;
            MaximumSize = textFormSize;
            BackColor = Color.White;
            TopMost = true;
            ShowInTaskbar = false;
            Location = new Point(Screensize.Width - textFormSize.Width - 200, Screensize.Height - textFormSize.Height);

            textbox = new TextBox()
            {
                Location = new Point((int)(textFormSize.Width * 0.1), 25),
                Size = new Size((int)(textFormSize.Width * 0.66), (int)(textFormSize.Height * 0.8))
            };



            submitbtn = new Button()
            {
                Location = new Point((int)(textFormSize.Width * 0.80), 20),
                Size = new Size(30, 30)
            };
            textbox.KeyDown += Textbox_KeyDown;


            submitbtn.Click += (sender, e) =>
            {
                userEventSubmit();
            };

            Controls.Add(textbox);
            Controls.Add(submitbtn);
        }

        private void Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                userEventSubmit();
                e.SuppressKeyPress = true;
            }   
        }

        private void userEventSubmit()
        {
            if (textbox.Text == "/trigger")
            {
                triggerPanel();
            }
            else
            {
                _userInput = textbox.Text;
                ShowReply();
                textbox.Clear();
            }
        }

        public void ShowReply()
        {
            ReplyForm replyer = new ReplyForm(_characterform, replyFormSize);
            replyer.ShowReply(_userInput);
        }


        private Button screentimeTrigger;
        private Button eventPostedTrigger;

        private void triggerPanel()
        {
            triggerForm = new Form()
            {
                Size = new Size(200, 300),
                Location = new Point(100, 100),
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                TopMost = true,
                ShowInTaskbar = false
            };

            triggerForm.Show();
            DragHelper.EnableDrag(triggerForm, triggerForm);

            screentimeTrigger = new Button()
            {
                Location = new Point(10, 10),
                Size = new Size(100, 50),
                Text = "Trigger Screen Time Reminder"
            };
            triggerForm.Controls.Add(screentimeTrigger);
            screentimeTrigger.Click += (sender, e) =>
            {
                ReplyForm replyer = new ReplyForm(_characterform, replyFormSize);
                replyer.ShowReply("System message: User's screentime has exceeded healthy limits, remind user to take a break");
                _characterform.ChangeState("Sleeping", true);
            };

            eventPostedTrigger = new Button()
            {
                Location = new Point(10, 70),
                Size = new Size(100, 50),
                Text = "Trigger Event Posted"
            };
            triggerForm.Controls.Add(eventPostedTrigger);
            eventPostedTrigger.Click += (sender, e) =>
            {
                ReplyForm replyer = new ReplyForm(_characterform, replyFormSize);
                replyer.ShowReply("System message: Company has posted an Event, promote it to the user");
            };

            eventPostedTrigger = new Button()
            {
                Location = new Point(110, 10),
                Size = new Size(80, 50),
                Text = "SSC - 10sec"
            };
            triggerForm.Controls.Add(eventPostedTrigger);

            eventPostedTrigger.Click += (sender, e) =>
            {

                Timer timer = new Timer();
                timer.Interval = 10000;
                timer.Tick += (s, args) =>
                {
                    timer.Stop();

                    ReplyForm replyer = new ReplyForm(_characterform, replyFormSize);
                    replyer.ShowReply("System message: User's screentime has exceeded healthy limits, remind user to take a break");
                };

                timer.Start();
            };
        }

    }
}
