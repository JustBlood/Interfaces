using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuadraticEquations
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        double a, b, c;

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBoxA.Text, out a) || !double.TryParse(textBoxB.Text, out b) || !double.TryParse(textBoxC.Text, out c))
            {
                MessageBox.Show("Некорректный ввод. Пожалуйста, введите числа.");
                return;
            }

            if (textBoxA.Text.Length > 13 || textBoxB.Text.Length > 13 || textBoxC.Text.Length > 13)
            {
                MessageBox.Show("Слишком большие числа");
            }
            
            labelRes.Text = SolveQuadraticEquation(a, b, c);
        }

        static string SolveQuadraticEquation(double a, double b, double c)
        {
            if (a == 0)
            {
                if (b == 0) return ("Уравнение не имеет действительных корней.");
                double root = -c / b;
                return ($"Уравнение имеет один корень: x = {root}");
            }

            double discriminant = b * b - 4 * a * c;

            if (discriminant > 0)
            {
                double root1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                double root2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                return($"Корни уравнения: x1 = {root1}, x2 = {root2}");
            }
            else if (discriminant == 0)
            {
                double root = -b / (2 * a);
                return($"Уравнение имеет один корень: x = {root}");
            }
            else
            {
                return ("Уравнение не имеет действительных корней.");
            }
        }
    }
}
