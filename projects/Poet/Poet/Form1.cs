using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Poet;

namespace Poet
{
    public partial class Form1 : Form
    {
        private SynonymDictionary synonymDictionary;
        private HFIntegrationService hFIntegrationService;
        string wordsFilePath = "C:\\Users\\User\\AllMine\\уник\\4 курс\\интерфейсы\\projects\\Poet\\russian.txt";
        Dictionary<(int, string), List<string>> wordsDictionary;

        public Form1()
        {
            InitializeComponent();
            LoadSynonymDictionary();
            wordsDictionary = LoadWordsFromFile(wordsFilePath);
            hFIntegrationService = new HFIntegrationService();
        }

        private void LoadSynonymDictionary()
        {
            string filePath = "C:\\Users\\User\\AllMine\\уник\\4 курс\\интерфейсы\\projects\\Poet\\dictionary.json";
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
            string modifiedPoem = ReplaceWordsInPoem(inputPoem, wordsDictionary);
            textBox2.Text = modifiedPoem;
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

        private void buttonRhyme_Click(object sender, EventArgs e)
        {
            string poem = textBoxInput.Text;
            if (string.IsNullOrEmpty(poem)) MessageBox.Show("Введите стихотворение");

            
        }

        static string ReplaceWordsInPoem(string poem, Dictionary<(int, string), List<string>> wordsDictionary)
        {
            var lines = poem.Split('\n');


            for (int i = 0; i < lines.Length; i++)
            {
                var words = lines[i].Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];
                    int syllableCount = CountSyllables(word);
                    string ending = GetEnding(word);

                    // Поиск подходящего слова
                    if (wordsDictionary.TryGetValue((syllableCount, ending), out var candidates))
                    {
                        // Замена на случайное подходящее слово
                        words[j] = candidates[new Random().Next(candidates.Count)];
                    }
                }
                lines[i] = string.Join(" ", words);
            }

            return string.Join("\n", lines);
        }

        static Dictionary<(int, string), List<string>> LoadWordsFromFile(string filePath)
        {
            var wordsDictionary = new Dictionary<(int, string), List<string>>();

            foreach (var line in File.ReadLines(filePath, Encoding.GetEncoding(1251)))
            {
                string word = line.Trim();

                // Определение количества слогов и окончания
                int syllableCount = CountSyllables(word);
                string ending = GetEnding(word);

                var key = (syllableCount, ending);
                if (!wordsDictionary.ContainsKey(key))
                {
                    wordsDictionary[key] = new List<string>();
                }
                wordsDictionary[key].Add(word);
            }

            return wordsDictionary;
        }

        static int CountSyllables(string word)
        {
            // Простой алгоритм для подсчета слогов
            return word.Count(c => "аеёиоуыэюяАЕЁИОУЫЭЮЯ".Contains(c));
        }

        static string GetEnding(string word)
        {
            // Получение окончания (последние 2-3 буквы)
            return word.Length >= 3 ? word.Substring(word.Length - 3) : word;
        }
    }
}
