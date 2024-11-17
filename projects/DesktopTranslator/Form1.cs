using System;
using Vosk;
using NAudio.Wave;
using GTranslatorAPI; // ���������, ��� ���� ����� ����������
using Newtonsoft.Json.Linq;
using System.Drawing.Drawing2D;

namespace DesktopTranslator
{
    public partial class Form1 : Form
    {
        private VoskRecognizer recognizer;
        private WaveInEvent waveIn; // ������ �� WaveInEvent
        private bool isRecognizing = false;
        private System.Windows.Forms.Timer animationTimer;

        Dictionary<string, Languages> dictLangs = new Dictionary<string, Languages>()
        {
            { "�������", Languages.ru },
            { "����������", Languages.en },
            { "�����������", Languages.fr },
            { "��������", Languages.de },
            { "���������", Languages.es },
            { "�����������", Languages.it },
            { "��������", Languages.ar },
            { "�����", Languages.hi },
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
                BackgroundImage = Image.FromFile("C:\\Users\\User\\AllMine\\����\\4 ����\\����������\\projects\\DesktopTranslator\\voice-recording.png"), // ������� ���� � �����������
                BackgroundImageLayout = ImageLayout.Stretch,
                Visible = false, // ������ ���������� ��������
                FlatStyle = FlatStyle.Flat
            };
            // �������� ������ ��� ���������� �����
            buttonVoiceInput.Click += buttonStartVoiceInput_Click;
            Controls.Add(buttonVoiceInput);
        }

        private void comboBoxSourceLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ����������� ������ ������ ��� �������� �����
            buttonVoiceInput.Visible = comboBoxSourceLang.SelectedItem.ToString() == "�������";
        }

        private void InitializeVosk()
        {
            // ������� ���� � ������
            string modelPath = @"C:\Users\User\AllMine\����\4 ����\����������\projects\DesktopTranslator\vosk-model-small-ru-0.22"; // �������� �� ���� � ����� ������
            Model model = new Model(modelPath);
            recognizer = new VoskRecognizer(model, 16000.0f);
        }

        private void InitializeAnimationTimer()
        {
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 200; // �������� 500 ��
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void buttonStartVoiceInput_Click(object sender, EventArgs e)
        {
            if (!isRecognizing)
            {
                // ������ ������������� ����
                StartRecognition();
            }
            else
            {
                // ���������� ������������� ����
                StopRecognition();
            }
        }

        private void StartRecognition()
        {
            // ����������� �������� ��� ������� �����
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
            animationTimer.Start(); // ��������� ������
        }

        private void StopRecognition()
        {
            // ���������� ������ �����
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null; // ����������� ������
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
                // �������� ��� ��������� �����
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
                // ��������� ������� ������ ��� �������� ������� ��������
                if (buttonVoiceInput.Size == new Size(30, 30))
                {
                    buttonVoiceInput.Size = new Size(33, 33); // ����������� ������
                }
                else
                {
                    buttonVoiceInput.Size = new Size(30, 30); // ��������� ������
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
            animationTimer?.Stop(); // ������������� ������
        }
    }
}
