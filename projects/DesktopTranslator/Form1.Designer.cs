namespace DesktopTranslator
{
    partial class Form1
    {
        private void InitializeComponent()
        {
            buttonTranslate = new Button();
            textBoxInput = new TextBox();
            label1 = new Label();
            comboBoxSourceLang = new ComboBox();
            textBoxOutput = new TextBox();
            comboBoxTargetLang = new ComboBox();
            buttonVoiceInput = new Button(); // Добавляем кнопку голосового ввода
            SuspendLayout();
            // 
            // buttonTranslate
            // 
            buttonTranslate.Location = new Point(29, 278);
            buttonTranslate.Margin = new Padding(3, 2, 3, 2);
            buttonTranslate.Name = "buttonTranslate";
            buttonTranslate.Size = new Size(102, 34);
            buttonTranslate.TabIndex = 0;
            buttonTranslate.Text = "Перевести";
            buttonTranslate.UseVisualStyleBackColor = true;
            buttonTranslate.BackColor = Color.LightSkyBlue; // Цвет кнопки
            buttonTranslate.ForeColor = Color.White; // Цвет текста на кнопке
            buttonTranslate.FlatStyle = FlatStyle.Flat; // Убираем стандартные границы
            buttonTranslate.Font = new Font("Arial", 10, FontStyle.Bold); // Шрифт
            buttonTranslate.Click += buttonTranslate_Click;
            // 
            // textBoxInput
            // 
            textBoxInput.Location = new Point(29, 94);
            textBoxInput.Margin = new Padding(3, 2, 3, 2);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.Size = new Size(255, 165);
            textBoxInput.TabIndex = 1;
            textBoxInput.BackColor = Color.WhiteSmoke; // Цвет фона
            textBoxInput.BorderStyle = BorderStyle.FixedSingle; // Границы
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 20);
            label1.Name = "label1";
            label1.Size = new Size(330, 15);
            label1.TabIndex = 2;
            label1.Text = "Введите текст и выберите исходный язык и язык перевода:";
            label1.ForeColor = Color.Black; // Цвет текста
            label1.Font = new Font("Arial", 12, FontStyle.Regular); // Шрифт
            // 
            // comboBoxSourceLang
            // 
            comboBoxSourceLang.FormattingEnabled = true;
            comboBoxSourceLang.Location = new Point(29, 53);
            comboBoxSourceLang.Margin = new Padding(3, 2, 3, 2);
            comboBoxSourceLang.Name = "comboBoxSourceLang";
            comboBoxSourceLang.Size = new Size(133, 23);
            comboBoxSourceLang.TabIndex = 3;
            comboBoxSourceLang.BackColor = Color.LightGray; // Цвет фона
            comboBoxSourceLang.ForeColor = Color.Black; // Цвет текста
            comboBoxSourceLang.SelectedIndexChanged += comboBoxSourceLang_SelectedIndexChanged; // Обработчик события
            // 
            // textBoxOutput
            // 
            textBoxOutput.Location = new Point(332, 94);
            textBoxOutput.Margin = new Padding(3, 2, 3, 2);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.Size = new Size(255, 165);
            textBoxOutput.TabIndex = 4;
            textBoxOutput.BackColor = Color.WhiteSmoke; // Цвет фона
            textBoxOutput.BorderStyle = BorderStyle.FixedSingle; // Границы
            // 
            // comboBoxTargetLang
            // 
            comboBoxTargetLang.FormattingEnabled = true;
            comboBoxTargetLang.Location = new Point(332, 53);
            comboBoxTargetLang.Margin = new Padding(3, 2, 3, 2);
            comboBoxTargetLang.Name = "comboBoxTargetLang";
            comboBoxTargetLang.Size = new Size(133, 23);
            comboBoxTargetLang.TabIndex = 5;
            comboBoxTargetLang.BackColor = Color.LightGray; // Цвет фона
            comboBoxTargetLang.ForeColor = Color.Black; // Цвет текста
            // 
            // buttonVoiceInput
            // 
            buttonVoiceInput.Size = new Size(30, 30);
            buttonVoiceInput.Location = new Point(comboBoxSourceLang.Right + 5, comboBoxSourceLang.Top);
            buttonVoiceInput.BackgroundImage = Image.FromFile("C:\\Users\\User\\AllMine\\уник\\4 курс\\интерфейсы\\projects\\DesktopTranslator\\voice-recording.png"); // Укажите путь к изображению
            buttonVoiceInput.BackgroundImageLayout = ImageLayout.Stretch;
            buttonVoiceInput.Visible = false; // Кнопка изначально невидима
            buttonVoiceInput.FlatStyle = FlatStyle.Flat;
            buttonVoiceInput.Click += buttonStartVoiceInput_Click;
            Controls.Add(buttonVoiceInput); // Добавляем кнопку на форму
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(comboBoxTargetLang);
            Controls.Add(textBoxOutput);
            Controls.Add(comboBoxSourceLang);
            Controls.Add(label1);
            Controls.Add(textBoxInput);
            Controls.Add(buttonTranslate);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Переводчик";
            BackColor = Color.FromArgb(240, 248, 255); // Цвет фона формы
            ResumeLayout(false);
            PerformLayout();
        }

        private Button buttonTranslate;
        private TextBox textBoxInput;
        private Label label1;
        private ComboBox comboBoxSourceLang;
        private TextBox textBoxOutput;
        private ComboBox comboBoxTargetLang;
        private Button buttonVoiceInput; // Объявление кнопки голосового ввода
    }
}
