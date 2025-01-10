using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;



namespace Echo_system
{
    public class AIChatbot
    {
        private readonly string apiKey;
        private string _charactername;
        private readonly string _memoryFilePath;

        private Dictionary<string, Action> commandDict = new Dictionary<string, Action>
        {
            { "/weather", GetWeather},
            { "/scheduler", GetScheduler}
        };


        public AIChatbot(string apiKey, string characterName)
        {
            this.apiKey = apiKey;
            _charactername = characterName;
            _memoryFilePath = Path.Combine(Application.StartupPath, "Characters" , characterName , "memory.json");

            Console.WriteLine("Memory file path: " + _memoryFilePath);
        }


        public class ChatMemory
        {
            public string Personality { get; set; }
            public string Instructions { get; set; }
            public List<dynamic> ConversationHistory { get; set; }
        }


        public async Task<string> GetResponseAsync(string userInput)
        {
                
            var memory = LoadMemory();


            if (memory == null)
            {
                memory = new ChatMemory
                {
                    Personality = "",
                    Instructions = "",
                    ConversationHistory = new List<dynamic>()
                };
            }

            if (memory.ConversationHistory.Count == 0)
            {
                memory.ConversationHistory.Insert(0, new { role = "system", content = memory.Personality });
                memory.ConversationHistory.Insert(0, new { role = "system", content = memory.Instructions });
            }

            memory.ConversationHistory.Add(new { role = "user", content = userInput });


            string response = await SendToOpenAIAsync(memory.ConversationHistory);  


            memory.ConversationHistory.Add(new { role = "assistant", content = response });

            SaveMemory(memory);

            return response;
        }

        private async Task<string> SendToOpenAIAsync(List<dynamic> conversationHistory)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");


                var content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        model = "gpt-3.5-turbo",
                        messages = conversationHistory,
                        temperature = 0.5,
                        max_tokens = 300
                    }),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();

                foreach (string command in commandDict.Keys)
                {

                    if (result.Contains(command))
                    {
                        Console.WriteLine($"Command found: {command}");

                        commandDict[command].Invoke();
                        result = result.Replace(command, "").Trim();
                    }
                }

                dynamic jsonResponse = JsonConvert.DeserializeObject(result);

                return jsonResponse.choices[0].message.content.ToString().Trim();
            }
        }

        private ChatMemory LoadMemory()
        {
            if (File.Exists(_memoryFilePath))
            {
                string json = File.ReadAllText(_memoryFilePath);
                return JsonConvert.DeserializeObject<ChatMemory>(json);
            }

            return null;
        }

        private void SaveMemory(ChatMemory memory)
        {
            string json = JsonConvert.SerializeObject(memory, Formatting.Indented);
            File.WriteAllText(_memoryFilePath, json);
        }


        private static void GetWeather()
        {
            Console.WriteLine("Fetching weather data...");
            MessageBox.Show("Showing Weather");
        }

        private static void GetScheduler()
        {
            Console.WriteLine("Fetching Schedule data...");
            MessageBox.Show("Showing Schedules");
        }
    }
}
  
