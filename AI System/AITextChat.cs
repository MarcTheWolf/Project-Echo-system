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

        private string _userInput;

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
            _userInput = textbox.Text;
            ShowReply();
            textbox.Clear();
        }

        public void ShowReply()
        {

            ReplyForm replyer = new ReplyForm(_characterform, new Size(200, 100));
            replyer.ShowReply(_userInput);
        }

    }
}
