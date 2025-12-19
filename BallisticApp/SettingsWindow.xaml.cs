using System;
using System.Globalization;
using System.Reflection;
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
            DefaultFieldInitializer.Apply(this);
            DrawPreviews();

            this.PreviewMouseDown += (s, e) =>
            {
                if (!(e.OriginalSource is TextBox))
                    FocusSink.Focus(); // moves focus away from any TextBox
            };
        }

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            //SettingsFieldUpdater.Apply(this, settings);
            DefaultFieldInitializer.Apply(this, settings);
            DrawPreviews();

            this.PreviewMouseDown += (s, e) =>
            {
                if (!(e.OriginalSource is TextBox))
                    FocusSink.Focus(); // moves focus away from any TextBox
            };
        }

        public static void LoadPreviousValues(SettingsWindow window,AppSettings settings) 
        {
            // Use CultureInfo.InvariantCulture to ensure consistent formatting
            var culture = CultureInfo.InvariantCulture;
            foreach (PropertyInfo prop in settings.Ballistics.GetType().GetProperties())
            {     
                if (window.FindName(prop.Name + "TextBox") is TextBox tb)
                {
                    object value = prop.GetValue(settings.Ballistics);
                    if (value != null)
                    {
                        //tb.Text = Convert.ToString(value, culture);
                        //DefaultTextBehavior.SetDefaultText(tb, Convert.ToString(value,culture));
                    }
                }
            }

        }

        private static double Parse(TextBox tb)
        => double.Parse(tb.Text, CultureInfo.InvariantCulture);

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
                    BallisticCoefficient = Parse(BallisticCoefficientTextBox),
                    BulletWeight = Parse(BulletWeightTextBox),
                    BulletDiameter = Parse(BulletDiameterTextBox),
                    BulletVelocity = Parse(MuzzleVelocityTextBox),
                    BarrelTwist = Parse(BarrelTwistTextBox),
                    SightHeight = Parse(SightHeightTextBox),
                    ZeroDistance = Parse(ZeroDistanceTextBox),
                    WindSpeed = Parse(WindSpeedTextBox),
                    WindDirection = Parse(WindDirectionTextBox),
                    Altitude = Parse(AltitudeTextBox),
                    Pressure = Parse(PressureTextBox),
                    Temperature = Parse(TemperatureTextBox),
                    Humidity = Parse(HumidityTextBox),
                    ShootingAngle = Parse(ShootingAngleTextBox),
                    Distance = Parse(DistanceTextBox),
                    TargetRadius = Parse(TargetRadiusTextBox),
                    DragModel = None.IsChecked == true ? BallisticSettings.DragModelEnum.None :
                    G1.IsChecked == true ? BallisticSettings.DragModelEnum.G1 :
                    BallisticSettings.DragModelEnum.G7
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

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            void Reset(TextBox tb)
            {
                string defaultValue = DefaultTextBehavior.GetDefaultText(tb);
                tb.Text = defaultValue;
                tb.Background = new SolidColorBrush(Color.FromRgb(235, 235, 235));
                DefaultTextBehavior.SetCurrentText(tb, defaultValue);
            }

            foreach (var tb in FindVisualChildren<TextBox>(this))
            {
                if (!string.IsNullOrEmpty(DefaultTextBehavior.GetDefaultText(tb)))
                    Reset(tb);
            }
        }

        // Helper method to find all children of a certain type
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T t)
                        yield return t;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
    }
}
