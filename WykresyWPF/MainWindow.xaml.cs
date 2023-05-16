using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WykresyWPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double[] values = { 1, 1, 1, 1 };
        Point[] points = new Point[4];
        public MainWindow()
        {
            InitializeComponent();
            DrawBarChart();
            DrawPointChart();
            DrawPieChar();
            btn1.IsChecked = true;
        }

        private void DrawBarChart()
        {
            double maximum = values.Max();
            int rectangleCount = 0;
            foreach (UIElement element in Bar.Children)
            {
                if (element is Rectangle)
                {
                    if (rectangleCount < values.Length)
                    {
                        double procent = (values[rectangleCount] * 100) / maximum;
                        double height = (300 * procent) / 100;
                        Rectangle rectangle = (Rectangle)element;
                        double newHeight = height;
                        rectangle.Height = newHeight;
                    }
                    rectangleCount++;
                }
            }
            DrawScale(Bar);
        }

        private void DrawLines()
        {
            Polyline polyline = Line.Children[4] as Polyline;
            polyline.Points.Clear();

            foreach (Point point in points)
            {
                polyline.Points.Add(point);
            }
        }

        private void DrawPointChart()
        {
            int pointsPositionCounter = 0;
            double maximum = values.Max();
            int rectangleCount = 0;

            foreach (UIElement element in Line.Children)
            {
                if (element is Ellipse)
                {
                    if (rectangleCount < values.Length)
                    {
                        double procent = (values[rectangleCount] * 100) / maximum;
                        double height = (300 * procent) / 100;
                        Ellipse rectangle = (Ellipse)element;
                        double newHeight = height;
                        Thickness t = new Thickness(-2.5, 0, 0, height);
                        rectangle.Margin = t;
                        points[rectangleCount].X = pointsPositionCounter;
                        points[rectangleCount].Y = 337.5 - height;
                        pointsPositionCounter += 129;
                    }
                    rectangleCount++;
                }
            }
            DrawScale(Line);
            DrawLines();
        }

        private void DrawPieChar()
        {
            double sum = values.Sum();
            double[] angles = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                angles[i] = 360 * values[i] / sum;
            }

            double centerX = 150;
            double centerY = 150;
            double radius = 100;
            double startAngle = 0;

            PathFigure[] pathFigures = { path1, path2, path3, path4 };
            LineSegment[] lineSegments = { line1, line2, line3, line4 };
            ArcSegment[] arcSegments = { arc1, arc2, arc3, arc4 };

            for (int i = 0; i < pathFigures.Length; i++)
            {
                pathFigures[i].StartPoint = new Point(centerX, centerY);
                lineSegments[i].Point = new Point(centerX + radius * Math.Cos(startAngle * Math.PI / 180), centerY + radius * Math.Sin(startAngle * Math.PI / 180));
                arcSegments[i].Point = new Point(centerX + radius * Math.Cos((startAngle + angles[i]) * Math.PI / 180), centerY + radius * Math.Sin((startAngle + angles[i]) * Math.PI / 180));
                arcSegments[i].Size = new Size(radius, radius);
                arcSegments[i].SweepDirection = SweepDirection.Clockwise;
                arcSegments[i].IsLargeArc = angles[i] > 180;

                startAngle += angles[i];
            }
        }

        private void DrawScale(Grid grid)
        {
            double maximum = values.Max();
            int labelsCount = 0;
            double[] array = new double[values.Length];
            foreach (UIElement element in grid.Children)
            {
                if (element is System.Windows.Controls.Label)
                {
                    double procent = (values[labelsCount] * 100) / maximum;
                    double height = (300 * procent) / 100;
                    array[labelsCount] = height;

                    System.Windows.Controls.Label label = (System.Windows.Controls.Label)element;
                    if (labelsCount < values.Length)
                    {
                        label.Height = height;
                        label.Content = values[labelsCount];
                    }
                    labelsCount++;
                }
            }

            if (grid.Name == "Line")
            {
                scale1.Height = array[0];
                scale2.Height = array[1];
                scale3.Height = array[2];
                scale4.Height = array[3];
            }
            else
            {
                scale1a.Height = array[0];
                scale2a.Height = array[1];
                scale3a.Height = array[2];
                scale4a.Height = array[3];
            }
        }

        private void name_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            foreach (UIElement element in Labels.Children)
            {
                if (element is System.Windows.Controls.Label label && label.Tag != null && label.Tag.ToString() == textBox.Tag.ToString())
                {
                    label.Content = textBox.Text;
                }
            }
        }

        private void value_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!String.IsNullOrEmpty(textBox.Text))
            {
                values[Convert.ToInt32(textBox.Tag) - 1] = Convert.ToDouble(textBox.Text);
                DrawBarChart();
                DrawPointChart();
                DrawPieChar();
            }
            else
            {
                values[Convert.ToInt32(textBox.Tag) - 1] = 0;
                DrawBarChart();
                DrawPointChart();
                DrawPieChar();
            }
        }

        private void value_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            double result;
            return double.TryParse(text, out result);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Bar.Visibility = Visibility.Collapsed;
            Line.Visibility = Visibility.Collapsed;
            Pie.Visibility = Visibility.Collapsed;

            //Check which button selected
            if (btn1.IsChecked == true)
            {
                Bar.Visibility = Visibility.Visible;
            }
            else if (btn2.IsChecked == true)
            {
                Line.Visibility = Visibility.Visible;
            }
            else if (btn3.IsChecked == true)
            {
                Pie.Visibility = Visibility.Visible;
            }
        }
    }
}

