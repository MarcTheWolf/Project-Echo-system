using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echo_system
{
    public partial class CalanderControl : UserControl
    {
        private string _day;
        private bool _selected = false;
        internal string date, weekday;
        private List<TaskItem> _tasklist;
        
        public CalanderControl(string day)
        {
            InitializeComponent();
            _day = day;
            label1.Text = day;
            checkBox1.Hide();
            panel2.BackColor = Color.Transparent;
            date = $"{_day}/{Calander._month}/{Calander._year}";
        }
        private void CalanderControl_Load(object sender, EventArgs e)
        {
            Sundays();
            GetTask();
        }
        private void Sundays()
        {
            try
            {
                DateTime day = DateTime.Parse(date);
                weekday = day.ToString("ddd");
                if (weekday == "Sun")
                {
                    label1.ForeColor = Color.DarkBlue;
                }
            }
            catch (Exception) { }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
        }

        private void panel1_Click(object sender, EventArgs e)
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

        internal bool Selected { get { return _selected; } }
        internal List<TaskItem> TaskList { get { return _tasklist; } }


        private void GetTask()
        {

            var taskManager = new TaskManager("tasks.json");


            try
            {
                DateTime currentDate = DateTime.Parse(date);


                _tasklist = taskManager.GetTasksForDate(currentDate);


                if (_tasklist.Count > 0)
                {
                    int yOffset = 25; 

                    foreach (var task in _tasklist)
                    {
                        TaskContainer taskContainer = new TaskContainer(task.Title, task.Time, task.Description);

                        taskContainer.Location = new Point(5, yOffset);
                        panel2.Controls.Add(taskContainer);
                        taskContainer.BringToFront();


                        yOffset += taskContainer.Height + 2;
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private class TaskContainer : Panel
        {
            private string _title;
            private string _description;
            private string _time;

            public TaskContainer(string title, string time, string description)
            {
                _title = title;
                _description = description;
                _time = time;


                this.Width = 110; 
                this.Height = 20;

                TimeSpan parsedTime = TimeSpan.Parse(_time);
                  
                Label titleLabel = new Label
                {
                    Text = $"{parsedTime.ToString(@"hh\:mm")}:  {_title}",
                    Location = new Point(0, 3),
                    Size = new Size(105, 20),
                    
                };
                this.Controls.Add(titleLabel);


            }
        }
    }

}
