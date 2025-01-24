using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Echo_system.Utility_Classes
{
    internal class NoteManager
    {
        private string _filePath;

        public NoteManager(string filepath)
        {
            _filePath = filepath;
        }

        public Dictionary<DateTime, NoteItem> GetAllNotes()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"The file '{_filePath}' does not exist.");
            }

            string jsonContent = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Dictionary<DateTime, NoteItem>>(jsonContent);
        }

    }

    public class NoteItem
    {
        public string Title { get; set; }
        public string LastModified { get; set; }
        public string Content { get; set; }
    }
}
