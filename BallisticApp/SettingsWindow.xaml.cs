using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static BallisticApp.ViewSettings;

namespace BallisticApp
{
    public partial class SettingsWindow : Window
    {

        public BallisticSettings Ballistics { get; private set; }
        public ViewSettings View { get; private set; }

        public SettingsWindow()
        {
            InitializeComponent();
            DrawPreviews();
        }

        private void DrawPreviews()
        {
            DrawCircularPreview();
            DrawAxesPreview();
        }

        private void DrawCircularPreview()
        {
            double center = CircularPreview.Width / 2;
            double radius = center - 5;

            var circle = new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(circle, center - radius);
            Canvas.SetTop(circle, center - radius);
            CircularPreview.Children.Add(circle);
        }

        private void DrawAxesPreview()
        {
            double width = AxesPreview.Width;
            double height = AxesPreview.Height;
            double centerX = width / 2;
            double centerY = height / 2;

            var vertical = new Line
            {
                X1 = centerX,
                Y1 = 0,
                X2 = centerX,
                Y2 = height,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            var horizontal = new Line
            {
                X1 = 0,
                Y1 = centerY,
                X2 = width,
                Y2 = centerY,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            AxesPreview.Children.Add(vertical);
            AxesPreview.Children.Add(horizontal);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ballistics = new BallisticSettings
                {
                    Distance = double.Parse(DistanceTextBox.Text, CultureInfo.InvariantCulture),
                    ScopeHeight = double.Parse(ScopeHeightTextBox.Text, CultureInfo.InvariantCulture),
                    BallisticCoefficient = double.Parse(BallisticCoeffTextBox.Text, CultureInfo.InvariantCulture),
                    DragModel = None.IsChecked == true ? BallisticSettings.DragModelEnum.None :
                    G1.IsChecked == true ? BallisticSettings.DragModelEnum.G1 :
                    BallisticSettings.DragModelEnum.G7,
                    TargetRadius = double.Parse(TargetRadiusTextBox.Text, CultureInfo.InvariantCulture),
                    BulletVelocity = double.Parse(VelocityTextBox.Text, CultureInfo.InvariantCulture),
                    ZeroDistance = double.Parse(ZeroDistanceTextBox.Text, CultureInfo.InvariantCulture),
                    WindDirection = double.Parse(WindDirectionTextBox.Text, CultureInfo.InvariantCulture),
                    WindSpeed = double.Parse(WindSpeedTextBox.Text, CultureInfo.InvariantCulture)
                };

                View = new ViewSettings
                {
                    TargetType = CircularOption.IsChecked == true
                    ? ViewSettings.TargetKind.Circular
                    : ViewSettings.TargetKind.Axes
                };

                DialogResult = true;
            }
            catch
            {
                MessageBox.Show("Invalid values");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
