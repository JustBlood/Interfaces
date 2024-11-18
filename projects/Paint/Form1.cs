using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {
        private bool drawing;
        private Point lastPoint;
        private Color color1 = Color.Black;
        private Color color2 = Color.White;
        private Bitmap canvas;
        private int lineWidth = 2;
        private string brushShape = "Pen"; // "Pen", "Line", "Rectangle", "Ellipse", "Fill", "Select"
        private bool eraserMode = false;
        private List<Shape> shapes = new List<Shape>();
        private Shape selectedShape = null;
        private Point selectionOffset;
        private LinkedList<Bitmap> undoStack = new LinkedList<Bitmap>();
        private bool isMovingText = false;

        public Form1()
        {
            //InitializeComponent();
            this.Text = "Enhanced Paint";
            this.DoubleBuffered = true;
            this.MouseDown += MouseDownEvent;
            this.MouseMove += MouseMoveEvent;
            this.MouseUp += MouseUpEvent;
            this.Paint += PaintEvent;
            // Настройка холста
            canvas = new Bitmap(800, 600);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
            }

            InitializeMenu();
            this.Width = 1000;
            this.Height = 700;
        }

        private void InitializeMenu()
        {
            // Создание меню
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Файл");
            ToolStripMenuItem color1Menu = new ToolStripMenuItem("Цвет 1");
            ToolStripMenuItem color2Menu = new ToolStripMenuItem("Цвет 2");
            ToolStripMenuItem loadImageMenu = new ToolStripMenuItem("Загрузить изображение");
            ToolStripMenuItem lineWidthMenu = new ToolStripMenuItem("Толщина линии");
            ToolStripMenuItem brushShapeMenu = new ToolStripMenuItem("Форма кисти");
            ToolStripMenuItem eraserMenu = new ToolStripMenuItem("Ластик");
            ToolStripMenuItem undoMenu = new ToolStripMenuItem("Отмена");
            ToolStripMenuItem saveMenu = new ToolStripMenuItem("Сохранить");
            ToolStripMenuItem clearMenu = new ToolStripMenuItem("Очистить холст");
            ToolStripMenuItem textMenu = new ToolStripMenuItem("Добавить текст");

            loadImageMenu.Click += LoadImage;
            color1Menu.Click += ChooseColor1;
            color2Menu.Click += ChooseColor2;
            lineWidthMenu.Click += ChooseLineWidth;
            brushShapeMenu.Click += ChooseBrushShape;
            eraserMenu.Click += ToggleEraser;
            undoMenu.Click += UndoLastAction;
            saveMenu.Click += SaveImage;
            clearMenu.Click += ClearCanvas;
            textMenu.Click += AddText;

            fileMenu.DropDownItems.Add(loadImageMenu);
            fileMenu.DropDownItems.Add(saveMenu);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(lineWidthMenu);
            menuStrip.Items.Add(brushShapeMenu);
            menuStrip.Items.Add(color1Menu);
            menuStrip.Items.Add(color2Menu);
            menuStrip.Items.Add(eraserMenu);
            menuStrip.Items.Add(undoMenu);
            menuStrip.Items.Add(clearMenu);
            menuStrip.Items.Add(textMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void MouseDownEvent(object sender, MouseEventArgs e)
        {
            SaveUndoState();
            drawing = true;
            lastPoint = e.Location;

            if (eraserMode)
            {
                brushShape = "Pen";
            }

            if (brushShape == "Select" && selectedShape != null)
            {
                if (selectedShape is TextShape textShape && textShape.Contains(e.Location))
                {
                    isMovingText = true;
                    selectionOffset = new Point(e.Location.X - textShape.Position.X, e.Location.Y - textShape.Position.Y);
                }
            }
            else if (brushShape == "Fill")
            {
                foreach (var shape in shapes)
                {
                    if (shape.Contains(e.Location))
                    {
                        shape.FillColor = color1;
                        return;
                    }
                }
                PerformFloodFill(e.Location);
            }
            else if (brushShape != "Pen")
            {
                selectedShape = new Shape
                {
                    StartPoint = lastPoint,
                    EndPoint = lastPoint,
                    Color = eraserMode ? this.BackColor : color1,
                    LineWidth = lineWidth,
                    ShapeType = brushShape
                };
            }

            this.Invalidate();
        }

        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                if (brushShape == "Pen")
                {
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.DrawLine(new Pen(eraserMode ? color2 : color1, lineWidth), lastPoint, e.Location);
                    }
                }
                else if (brushShape == "Rectangle" || brushShape == "Ellipse" || brushShape == "Line")
                {
                    selectedShape.EndPoint = e.Location;
                    this.Invalidate(); // Перерисовать форму
                }
                if (eraserMode)
                {
                    // Проверяем, не попадает ли точка на текстовые фигуры
                    for (int i = shapes.Count - 1; i >= 0; i--)
                    {
                        if (shapes[i].Contains(e.Location))
                        {
                            if (shapes[i].Equals(selectedShape)) selectedShape = null;
                            shapes.RemoveAt(i); // Удаляем текст, если он попадает под ластик
                        }
                    }
                }

                lastPoint = e.Location;
                this.Invalidate();
            }

            if (isMovingText && selectedShape is TextShape textShape)
            {
                textShape.Position = new Point(e.Location.X - selectionOffset.X, e.Location.Y - selectionOffset.Y);
                this.Invalidate();
            }
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            drawing = false;
            isMovingText = false;
            if (!eraserMode && selectedShape != null && !shapes.Contains(selectedShape))
            {
                shapes.Add(selectedShape);
            }
            this.Invalidate();
        }

        private void PaintEvent(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(canvas, 0, 0);
            foreach (var shape in shapes)
            {
                shape.Draw(e.Graphics);
            }

            if (selectedShape != null)
            {
                selectedShape.Draw(e.Graphics);
            }
        }

        private void PerformFloodFill(Point start)
        {
            Color targetColor = canvas.GetPixel(start.X, start.Y);
            if (targetColor == (eraserMode ? color2 : color1)) return;

            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                if (current.X < 0 || current.X >= canvas.Width || current.Y < 0 || current.Y >= canvas.Height) continue;
                if (canvas.GetPixel(current.X, current.Y) != targetColor) continue;

                canvas.SetPixel(current.X, current.Y, eraserMode ? color2 : color1);
                if (!queue.Contains(new Point(current.X - 1, current.Y))) 
                {
                    queue.Enqueue(new Point(current.X - 1, current.Y));
                }
                if (!queue.Contains(new Point(current.X + 1, current.Y)))
                {
                    queue.Enqueue(new Point(current.X + 1, current.Y));
                }
                if (!queue.Contains(new Point(current.X, current.Y - 1)))
                {
                    queue.Enqueue(new Point(current.X, current.Y - 1));
                }
                if (!queue.Contains(new Point(current.X, current.Y + 1)))
                {
                    queue.Enqueue(new Point(current.X, current.Y + 1));
                }
            }

            this.Invalidate();
        }

        private void SaveUndoState()
        {
            Bitmap copy = new Bitmap(canvas);
            undoStack.AddLast(copy);
            if (undoStack.Count > 30) {  }
        }

        private void UndoLastAction(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                canvas = undoStack.Last();
                undoStack.RemoveLast();
                this.Invalidate();
            }
        }

        private void ChooseColor1(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                color1 = dialog.Color;
            }
        }

        private void ChooseColor2(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                color2 = dialog.Color;
            }
        }

        private void ClearCanvas(object sender, EventArgs e)
        {
            shapes.Clear();
            selectedShape = null;
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
            }
            this.Invalidate();
        }

        private void LoadImage(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Bitmap loadedImage = new Bitmap(openFileDialog.FileName))
                {
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.DrawImage(loadedImage, 0, 0, canvas.Width, canvas.Height);
                    }
                }
                this.Invalidate();
            }
        }

        private void ChooseLineWidth(object sender, EventArgs e)
        {
            using (Form lineWidthForm = new Form())
            {
                lineWidthForm.Height = 130;
                lineWidthForm.Width = 400;
                lineWidthForm.Text = "Выбор толщины линии";
                NumericUpDown numericUpDown = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = 20,
                    Value = lineWidth,
                    Dock = DockStyle.Fill
                };
                Button okButton = new Button
                {
                    Text = "OK",
                    Dock = DockStyle.Bottom
                };
                okButton.Click += (s, ev) =>
                {
                    lineWidth = (int)numericUpDown.Value;
                    lineWidthForm.Close();
                };

                lineWidthForm.Controls.Add(numericUpDown);
                lineWidthForm.Controls.Add(okButton);
                lineWidthForm.ShowDialog();
            }
        }

        private void ChooseBrushShape(object sender, EventArgs e)
        {
            using (Form brushShapeForm = new Form())
            {
                brushShapeForm.Height = 130;
                brushShapeForm.Width = 400;
                brushShapeForm.Text = "Выбор формы кисти";
                ComboBox comboBox = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                comboBox.Items.AddRange(new string[] { "Pen", "Select", "Line", "Rectangle", "Ellipse", "Fill" });
                comboBox.SelectedItem = brushShape;

                Button okButton = new Button
                {
                    Text = "OK",
                    Dock = DockStyle.Bottom
                };
                okButton.Click += (s, ev) =>
                {
                    brushShape = comboBox.SelectedItem.ToString();
                    brushShapeForm.Close();
                };

                brushShapeForm.Controls.Add(comboBox);
                brushShapeForm.Controls.Add(okButton);
                brushShapeForm.ShowDialog();
            }
        }

        private void ToggleEraser(object sender, EventArgs e)
        {
            eraserMode = !eraserMode;
            (sender as ToolStripMenuItem).Text = eraserMode ? "Режим рисования" : "Ластик";
        }

        private void SaveImage(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    // Отрисовываем все фигуры
                    foreach (var shape in shapes)
                    {
                        shape.Draw(g); // Рисуем каждую фигуру на canvas
                    }
                }

                // Сохраняем canvas в выбранный файл
                canvas.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void AddText(object sender, EventArgs e)
        {
            using (Form textForm = new Form())
            {
                textForm.Text = "Добавить текст";
                Font choosedFont = new Font("Arial", 12);
                TextBox textBox = new TextBox { Dock = DockStyle.Fill };
                Button okButton = new Button { Text = "OK", Dock = DockStyle.Bottom };
                Button fontButton = new Button { Text = "Выбрать Шрифт", Dock = DockStyle.Top };

                fontButton.Click += (s, ev) =>
                {
                    FontDialog fontDialog = new FontDialog();
                    if (fontDialog.ShowDialog() == DialogResult.OK)
                    {
                        choosedFont = fontDialog.Font;
                        this.Invalidate();
                    }
                };

                okButton.Click += (s, ev) =>
                {
                    string text = textBox.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        TextShape textShape = new TextShape
                        {
                            Text = text,
                            Position = lastPoint.Equals(new Point(0,0)) ? new Point(canvas.Width / 2, canvas.Height / 2) : lastPoint,
                            Color = color1,
                            Font = choosedFont
                        };
                        brushShape = "Select";
                        shapes.Add(textShape);
                        this.Invalidate();
                        selectedShape = textShape;
                    }
                    textForm.Close();
                };

                textForm.Controls.Add(textBox);
                textForm.Controls.Add(okButton);
                textForm.Controls.Add(fontButton);
                textForm.ShowDialog();
            }
        }

        public class Shape
        {
            public Point StartPoint { get; set; }
            public Point EndPoint { get; set; }
            public Color Color { get; set; }
            public Color FillColor { get; set; }
            public int LineWidth { get; set; }
            public string ShapeType { get; set; }

            public virtual void Draw(Graphics g)
            {
                using (Pen pen = new Pen(Color, LineWidth))
                {
                    if (ShapeType == "Line")
                    {
                        g.DrawLine(pen, StartPoint, EndPoint);
                    }
                    else if (ShapeType == "Rectangle")
                    {
                        g.FillRectangle(new SolidBrush(FillColor), Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                        g.DrawRectangle(pen, Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                    }
                    else if (ShapeType == "Ellipse")
                    {
                        g.FillEllipse(new SolidBrush(FillColor), Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                        g.DrawEllipse(pen, Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y));
                    }
                }
            }

            public virtual bool Contains(Point p)
            {
                return new Rectangle(Math.Min(StartPoint.X, EndPoint.X), Math.Min(StartPoint.Y, EndPoint.Y),
                            Math.Abs(StartPoint.X - EndPoint.X), Math.Abs(StartPoint.Y - EndPoint.Y)).Contains(p);
            }
        }

        public class TextShape : Shape
        {
            public Point Position { get; set; }
            public string Text { get; set; }
            public Font Font { get; set; }
            public Color Color { get; set; }

            public override void Draw(Graphics g)
            {
                using (Brush brush = new SolidBrush(Color))
                {
                    g.DrawString(Text, Font, brush, Position);
                }
            }

            public Rectangle GetBoundingRectangle()
            {
                using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    SizeF size = g.MeasureString(Text, Font);
                    return new Rectangle(Position, size.ToSize());
                }
            }

            public override bool Contains(Point p)
            {
                return GetBoundingRectangle().Contains(p);
            }
        }
    }
}
