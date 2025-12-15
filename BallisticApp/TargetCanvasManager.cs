using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
namespace BallisticApp
{
    internal class TargetCanvasManager
    {
        private readonly Canvas canvas;
        private readonly int totalRings = 10;
        private double centerX;
        private double centerY;
        private AppSettings settings;
        private Shot shot;
        private BallisticCalculator calculator;
        //public double Center => center;
        public TargetCanvasManager(Canvas canvas, AppSettings settings, Shot shot, BallisticCalculator calculator)
        {
            this.canvas = canvas;
            this.settings = settings;
            this.centerX = canvas.ActualWidth / 2;
            this.centerY = canvas.ActualHeight / 2;
            this.shot = shot;
            this.calculator = calculator;
            DrawResult(settings.View.TargetType, this.shot, this.calculator);
        }
        public void DrawResult(ViewSettings.TargetKind targetType, Shot shot, BallisticCalculator calculator)
        {
            switch (targetType)
            {
                case ViewSettings.TargetKind.Circular:
                    DrawCircularResult(shot);
                    break;
                case ViewSettings.TargetKind.Axes:
                    DrawAxesResult(shot, calculator);
                    break;
            }
        }
        private void DrawCircularResult(Shot shot)
        {
            DrawCircle();
            double maxDropMeters = shot.verticalDrop;
            double canvasHeightNeeded = center + MetersToPixels(maxDropMeters, settings.Ballistics.TargetRadius) + 20;
            if (canvasHeightNeeded > canvas.Height) canvas.Height = canvasHeightNeeded;
            // Draw MOA ticks along vertical axis
            double tickIntervalMeters = calculator.ComputeMOADistance();
            for (double i = 0; i <= maxDropMeters; i += tickIntervalMeters)
            {
                double yPos = center + MetersToPixels(i, settings.Ballistics.TargetRadius);
                var tick = new Line
                {
                    X1 = center - 20,
                    Y1 = yPos,
                    X2 = center + 20,
                    Y2 = yPos,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 1
                };
                canvas.Children.Add(tick);
            }
            // Render the shot
            double shotY = center + MetersToPixels(shot.verticalDrop, settings.Ballistics.TargetRadius);
            var hit = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.Red
            }
            ;
            Canvas.SetLeft(hit, center - hit.Width / 2);
            Canvas.SetTop(hit, shotY - hit.Height / 2);
            canvas.Children.Add(hit);
            //AddShot(shot,settings.Ballistics.TargetRadius ); 
            //AddLine(settings.Ballistics.TargetRadius);
        }
        private void DrawAxesResult(Shot shot, BallisticCalculator calculator)
        {
            canvas.Children.Clear();
            center = canvas.ActualWidth / 2;
            // Determine maximum drop to render full MOA ruler
            double maxDropMeters = shot.verticalDrop; double canvasHeightNeeded = center + MetersToPixels(maxDropMeters, settings.Ballistics.TargetRadius) + 20; if (canvasHeightNeeded > canvas.Height) canvas.Height = canvasHeightNeeded;
            // Draw vertical and horizontal axes
            var verticalLine = new Line
            {
                X1 = center,
                Y1 = 0,
                X2 = center,
                Y2 = canvas.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            }
            ;
            var horizontalLine = new Line
            {
                X1 = 0,
                Y1 = center,
                X2 = canvas.ActualWidth,
                Y2 = center,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            canvas.Children.Add(verticalLine);
            canvas.Children.Add(horizontalLine);
            // Draw MOA ticks along vertical axis
            double tickIntervalMeters = calculator.ComputeMOADistance();
            for (double i = 0; i <= maxDropMeters; i += tickIntervalMeters)
            {
                double yPos = center + MetersToPixels(i, settings.Ballistics.TargetRadius);
                var tick = new Line
                {
                    X1 = center - 20,
                    Y1 = yPos,
                    X2 = center + 20,
                    Y2 = yPos,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 1
                };
                canvas.Children.Add(tick);
            }
            // Render the shot
            double shotY = center + MetersToPixels(shot.verticalDrop, settings.Ballistics.TargetRadius);
            var hit = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.Red
            }
            ;
            Canvas.SetLeft(hit, center - hit.Width / 2);
            Canvas.SetTop(hit, shotY - hit.Height / 2);
            canvas.Children.Add(hit);
        }
        private void DrawCircle()
        {
            canvas.Children.Clear();
            center = canvas.ActualWidth / 2;
            double radiusIncrement = center / totalRings;
            for (int i = totalRings; i > = 1; i--)
            {
                double diameter = radiusIncrement * i * 2;
                var ring = new Ellipse
                {
                    Width = diameter,
                    Height = diameter,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(ring, center - diameter / 2);
                Canvas.SetTop(ring, center - diameter / 2);
                canvas.Children.Add(ring);
            }
            for (int i = 1; i < totalRings; i++)
            {
                double d = radiusIncrement * (i + 0.5);
                int num = totalRings - i;
                AddNumber(num.ToString(), center - d, center);
                AddNumber(num.ToString(), center + d, center);
                AddNumber(num.ToString(), center, center - d);
                AddNumber(num.ToString(), center, center + d);
            }
            AddNumber("10", center, center);
        }
        private void AddNumber(string text, double x, double y, double fontSize = 14)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                FontWeight = System.Windows.FontWeights.Bold
            };
            tb.Measure(
              new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity)
            );
            var size = tb.DesiredSize;
            Canvas.SetLeft(tb, x - size.Width / 2);
            Canvas.SetTop(tb, y - size.Height / 2);
            canvas.Children.Add(tb);
        }
        public double MetersToPixels(double meters, double targetRadiusCm) => meters * (
          canvas.ActualHeight / (targetRadiusCm * 2 / 100)
        );
        public double PixelsToMeters(double pixels, double targetRadiusCm) => pixels * (
          (targetRadiusCm * 2 / 100) / canvas.ActualHeight
        );
        public void AddShot(Shot shot, double targetRadiusCm)
        {
            double y = center + MetersToPixels(shot.verticalDrop, targetRadiusCm);
            if (y > canvas.Height) canvas.Height = y + 20;
            var hit = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(hit, center - hit.Width / 2);
            Canvas.SetTop(hit, y - hit.Height / 2);
            canvas.Children.Add(hit);
        }
        public void AddLine(double targetRadiusCm)
        {
            double length = canvas.Height - 20;
            double lengthInMeters = PixelsToMeters(length, targetRadiusCm);
            var line = new Line
            {
                X1 = center,
                Y1 = center,
                X2 = center,
                Y2 = length,
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };
            canvas.Children.Add(line);
            for (double i = 0; i < lengthInMeters; i = i + 0.1)
            {
                var tickline = new Line
                {
                    X1 = center - 20,
                    Y1 = center + +MetersToPixels(i, targetRadiusCm),
                    X2 = center + 20,
                    Y2 = center + +MetersToPixels(i, targetRadiusCm),
                    Stroke = Brushes.Orange,
                    StrokeThickness = 2
                };
                canvas.Children.Add(tickline);
            }
        }
    }
}