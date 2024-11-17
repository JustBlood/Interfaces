using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using HuggingFace;

namespace Poet
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class SynonymDictionary
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublishedAt { get; set; }
        public List<WordEntry> WordList { get; set; }

        public SynonymDictionary LoadSynonyms(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<SynonymDictionary>(json);
        }
    }

    public class WordEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public List<string> Synonyms { get; set; } = new List<string>();
    }

    public class HFIntegrationService
    {
        private readonly HuggingFaceApi api;

        public HFIntegrationService()
        {
            api = new HuggingFaceApi("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");
        }

        public async Task<string> TestRequest(string poem)
        {
            bool isRussian = poem.Any(c => c >= 'А' && c <= 'я');
            string prefix = isRussian ? "Перепиши стих другими словами, сохраняя рифму, на русском языке: " : "Rewrite the verse by another words, keeping the rhyme: ";
            string inputs = prefix + poem;
            var response = await api.GenerateTextAsync(
                "mistralai/Mixtral-8x7B-Instruct-v0.1",
                new GenerateTextRequest
                {
                    Inputs = inputs,
                    Parameters = new GenerateTextRequestParameters
                    {
                        MaxNewTokens = 500,
                    },
                });

            // Проверяем, есть ли результаты и извлекаем текст
            if (response != null && response.Count > 0)
            {
                // Здесь берем первый результат (или обработайте логику по-другому, если нужно)
                return response[0].GeneratedText.Replace(inputs, "").Replace("\n", "\r\n"); // Предполагая, что у GenerateTextResponseValue есть свойство GeneratedText
            }

            return string.Empty; // Возвращаем пустую строку, если нет результатов
        }
    }

}
