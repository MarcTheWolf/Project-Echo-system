using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Echo_system
{
    internal class TaskManager
    {
        private readonly string _filePath;

        public TaskManager(string filePath)
        {
            _filePath = Path.Combine(Application.StartupPath, filePath);
        }

        public Dictionary<DateTime, List<TaskItem>> GetAllTasks() 
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"The file '{_filePath}' does not exist.");
            }

            string jsonContent = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Dictionary<DateTime, List<TaskItem>>>(jsonContent);
        }

        public void AddTask(DateTime date, string time, string title, string description)
        {
            var tasks = GetAllTasks();


            if (!tasks.ContainsKey(date))
            {
                tasks[date] = new List<TaskItem>();
            }

            tasks[date].Add(new TaskItem
            {
                Time = time,
                Title = title,
                Description = description
            });

            string updatedJson = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, updatedJson);
        }

        public List<TaskItem> GetTasksForDate(DateTime date)
        {
            var tasks = GetAllTasks();


            if (tasks.ContainsKey(date))
            {

                return tasks[date]
                    .OrderBy(task => TimeSpan.Parse(task.Time))
                    .ToList();
            }


            return new List<TaskItem>();
        }
    }
    public class TaskItem
    {
        public string Time { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }



}
