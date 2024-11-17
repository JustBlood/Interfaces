using System;
using Vosk;
using NAudio.Wave;
using GTranslatorAPI; // Убедитесь, что этот пакет установлен
using Newtonsoft.Json.Linq;
using System.Drawing.Drawing2D;

namespace DesktopTranslator
{
    public partial class Form1 : Form
    {
        private VoskRecognizer recognizer;
        private WaveInEvent waveIn; // Ссылка на WaveInEvent
        private bool isRecognizing = false;
        private System.Windows.Forms.Timer animationTimer;

        Dictionary<string, Languages> dictLangs = new Dictionary<string, Languages>()
        {
            { "Русский", Languages.ru },
            { "Английский", Languages.en },
            { "Французский", Languages.fr },
            { "Немецкий", Languages.de },
            { "Испанский", Languages.es },
            { "Итальянский", Languages.it },
            { "Арабский", Languages.ar },
            { "Хинди", Languages.hi },
        };

        public Form1()
        {
            InitializeComponent();
            InitializeVosk();
            InitializeAnimationTimer();
            InitializeVoiceInputButton();
            comboBoxSourceLang.Items.AddRange(dictLangs.Keys.ToArray());
            comboBoxSourceLang.SelectedIndex = 0;
            comboBoxTargetLang.Items.AddRange(dictLangs.Keys.ToArray());
            comboBoxTargetLang.SelectedIndex = 1;
        }

        private void InitializeVoiceInputButton()
        {
            buttonVoiceInput = new Button
            {
                Size = new Size(30, 30),
                Location = new Point(comboBoxSourceLang.Right + 5, comboBoxSourceLang.Top),
                BackgroundImage = Image.FromFile("C:\\Users\\User\\AllMine\\уник\\4 курс\\интерфейсы\\projects\\DesktopTranslator\\voice-recording.png"), // Укажите путь к изображению
                BackgroundImageLayout = ImageLayout.Stretch,
                Visible = false, // Кнопка изначально невидима
                FlatStyle = FlatStyle.Flat
            };
            // Создание кнопки для голосового ввода
            buttonVoiceInput.Click += buttonStartVoiceInput_Click;
            Controls.Add(buttonVoiceInput);
        }

        private void comboBoxSourceLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Отображение кнопки только для русского языка
            buttonVoiceInput.Visible = comboBoxSourceLang.SelectedItem.ToString() == "Русский";
        }

        private void InitializeVosk()
        {
            // Укажите путь к модели
            string modelPath = @"C:\Users\User\AllMine\уник\4 курс\интерфейсы\projects\DesktopTranslator\vosk-model-small-ru-0.22"; // Замените на путь к вашей модели
            Model model = new Model(modelPath);
            recognizer = new VoskRecognizer(model, 16000.0f);
        }

        private void InitializeAnimationTimer()
        {
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 200; // Интервал 500 мс
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void buttonStartVoiceInput_Click(object sender, EventArgs e)
        {
            if (!isRecognizing)
            {
                // Начать распознавание речи
                StartRecognition();
            }
            else
            {
                // Остановить распознавание речи
                StopRecognition();
            }
        }

        private void StartRecognition()
        {
            // Используйте микрофон для захвата аудио
            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1)
            };

            waveIn.DataAvailable += (s, a) =>
            {
                if (recognizer.AcceptWaveform(a.Buffer, a.BytesRecorded))
                {
                    var result = recognizer.Result();
                    UpdateOutputTextBox(result);
                }
            };

            waveIn.StartRecording();
            isRecognizing = true;
            animationTimer.Start(); // Запускаем таймер
        }

        private void StopRecognition()
        {
            // Остановите захват аудио
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null; // Освобождаем ресурс
            }

            recognizer.Reset();
            isRecognizing = false;
        }

        private void UpdateOutputTextBox(string result)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateOutputTextBox(result)));
            }
            else
            {
                // Обновите ваш текстовый вывод
                var json = JObject.Parse(result);
                if (json.ContainsKey("text"))
                {
                    textBoxInput.Text = json["text"].ToString();
                }
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (isRecognizing)
            {
                // Изменение размера кнопки для создания эффекта анимации
                if (buttonVoiceInput.Size == new Size(30, 30))
                {
                    buttonVoiceInput.Size = new Size(33, 33); // Увеличиваем размер
                }
                else
                {
                    buttonVoiceInput.Size = new Size(30, 30); // Уменьшаем размер
                }
            }
        }

        private async void buttonTranslate_Click(object sender, EventArgs e)
        {
            string inputText = textBoxInput.Text;
            string sourseLanguage = comboBoxSourceLang.SelectedItem.ToString();
            string targetLanguage = comboBoxTargetLang.SelectedItem.ToString();

            GTranslatorAPIClient translator = new GTranslatorAPIClient();
            var result = await translator.TranslateAsync(dictLangs[sourseLanguage], dictLangs[targetLanguage], inputText);

            textBoxOutput.Text = result == null ? "" : result.TranslatedText;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            recognizer?.Dispose();
            animationTimer?.Stop(); // Останавливаем таймер
        }
    }
}
