using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;

namespace Echo_system.AI_System
{
    public partial class ReplyForm : Form
    {
        private CharacterForm _characterForm;
        private Size _size;
        private Label replyLabel;
        private Timer timer;
        private Timer fps;
        private AIChatbot AIChatbot;

        public ReplyForm(CharacterForm characterForm, Size size)
        {
            InitializeComponent();
            _characterForm = characterForm;
            _size = size;

            this.Size = _size;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                Math.Max(0, characterForm.PictureBoxLocation.X - _size.Width),
                Math.Max(0, characterForm.PictureBoxLocation.Y)
            );
            this.ShowInTaskbar = false;
            TopMost = true;


            replyLabel = new Label()
            {
                Size = new Size(this.ClientSize.Width - 10, this.ClientSize.Height - 10),
                Location = new Point(5, 5),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.None,
                Text = "", 
            };

            this.Controls.Add(replyLabel);


            timer = new Timer
            {
                Interval = 5000 
            };
            timer.Tick += Timer_Tick;


            fps = new Timer
            {
                Interval = 1
            };
            fps.Tick += UpdateLocation;
            fps.Start();

            AIChatbot = new AIChatbot(Properties.Resources.ApiKey, _characterForm.CharacterName);
        }

        /// <summary>
        /// Displays the form with the specified text.
        /// </summary>
        /// <param name="text">The text to display in the reply label.</param>
        public async void ShowReply(string text)
        {
            string outputText = await ProcessOutput(text);
            replyLabel.Text = outputText;
            this.Show();
            _characterForm.ChangeState("Moving", false);
            timer.Start();
        }

        /// <summary>
        /// Passes the input text to the AIChatbot class, returns the aichatbot output
        /// </summary>
        /// <param name="inputtext"></param>
        /// <returns></returns>
        private async Task<string> ProcessOutput(string inputText)
        {
            string outputText = await AIChatbot.GetResponseAsync(inputText);
            return outputText;
        }
        private void UpdateLocation(object sender, EventArgs e)
        {
            this.Location = new Point(
                Math.Max(0, _characterForm.PictureBoxLocation.X - _size.Width),
                Math.Max(0, _characterForm.PictureBoxLocation.Y)
            );
        }

        /// <summary>
        /// Timer tick event handler to close the form and stop the timer.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            this.Close();
        }
    }


}
