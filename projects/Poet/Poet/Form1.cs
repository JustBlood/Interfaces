using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Poet;

namespace Poet
{
    public partial class Form1 : Form
    {
        private SynonymDictionary synonymDictionary;
        private HFIntegrationService hFIntegrationService;

        public Form1()
        {
            InitializeComponent();
            LoadSynonymDictionary();
            hFIntegrationService = new HFIntegrationService();
        }

        private void LoadSynonymDictionary()
        {
            string filePath = "C:\\Users\\User\\AllMine\\уник\\4 курс\\интерфейсы\\projects\\Poet\\dictionary.json"; // Укажите путь к вашему файлу
            synonymDictionary = LoadSynonyms(filePath);
        }

        private SynonymDictionary LoadSynonyms(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<SynonymDictionary>(json);
        }

        private async void buttonGenerate_Click(object sender, EventArgs e)
        {
            string inputPoem = textBoxInput.Text;
            string resultPoem = await hFIntegrationService.TestRequest(inputPoem);
            textBox1.Text = ReplaceWithSynonyms(inputPoem, synonymDictionary);
            textBoxOutput.Text = resultPoem.TrimStart('\n', '\r');
        }

        public string ReplaceWithSynonyms(string input, SynonymDictionary dictionary)
        {
            var words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i].TrimEnd('.', ',', '!', '?');
                var entry = dictionary.WordList.FirstOrDefault(w => w.Name.Equals(word, StringComparison.OrdinalIgnoreCase));

                if (entry != null && entry.Synonyms.Count > 0)
                {
                    var random = new Random();
                    words[i] = entry.Synonyms[random.Next(entry.Synonyms.Count)].Split(';').First();
                }
            }
            return string.Join(" ", words);
        }
    }
}
