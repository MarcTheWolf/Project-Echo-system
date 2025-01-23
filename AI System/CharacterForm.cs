
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;




namespace Echo_system.AI_System
{
    public class CharacterForm : Form
    {
        private List<string> _stateList = new List<string>() { "Idle", "Moving" };

        private string characterName;
        private string currentState;
        private Timer frameTimer;
        private PictureBox pictureBox;
        private string[] frameFiles;
        private int currentFrameIndex;
        private string framesDirectory;
        private bool loopAnimation = true;

        public CharacterForm(string characterName, Size formSize, Size characterSize, Size screenSize)
        {
            this.characterName = characterName;
            this.currentState = "Idle";
            this.StartPosition = FormStartPosition.Manual;
            this.Size = formSize;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.Location = new Point(screenSize.Width - formSize.Width, screenSize.Height - formSize.Height);
            this.TopMost = true;
            this.ShowInTaskbar = false;


            pictureBox = new PictureBox
            {
                Size = characterSize,
                Location = new Point(screenSize - characterSize),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(pictureBox);
            DragHelper.EnableDrag(pictureBox, pictureBox);


            frameTimer = new Timer { Interval = 100 };
            frameTimer.Tick += FrameTimer_Tick;

                 
            framesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "characters", characterName, currentState);
            LoadFramesForState(currentState);
        }

        public Point PictureBoxLocation { get { return pictureBox.Location; } }
        public string CharacterName { get {  return characterName; } }
            
        private void LoadFramesForState(string state)
        {
            currentState = state;
            framesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "characters", characterName, currentState);
            if (Directory.Exists(framesDirectory))
            {
                frameFiles = Directory.GetFiles(framesDirectory, "*.*");

                Array.Sort(frameFiles, (x, y) =>
                {

                    int numX = ExtractNumberFromFileName(x);
                    int numY = ExtractNumberFromFileName(y);


                    return numX.CompareTo(numY);
                });

                currentFrameIndex = 0;

                frameTimer.Start();
            }
            else
            {
                MessageBox.Show($"No frames found for state: {state}");
            }
        }

        private void FrameTimer_Tick(object sender, EventArgs e)
        {
            if (frameFiles.Length > 0)
            {
                string currentFrame = frameFiles[currentFrameIndex];
                pictureBox.Image?.Dispose();
                pictureBox.Image = Image.FromFile(currentFrame);


                currentFrameIndex = (currentFrameIndex + 1) % frameFiles.Length;


                if (!loopAnimation && currentFrameIndex == 0)
                {
                    ChangeState("Idle");
                }
            } 
        }


        public void ChangeState(string state, bool loop = true)
        {
            if (currentState != state)
            {
                frameTimer.Stop();
                loopAnimation = loop;
                LoadFramesForState(state);
            }
        }




        private int ExtractNumberFromFileName(string fileName)
        {

            var match = System.Text.RegularExpressions.Regex.Match(Path.GetFileNameWithoutExtension(fileName), @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CharacterForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "CharacterForm";
            this.Load += new System.EventHandler(this.CharacterForm_Load);
            this.ResumeLayout(false);

        }

        private void CharacterForm_Load(object sender, EventArgs e)
        {

        }
    }

}