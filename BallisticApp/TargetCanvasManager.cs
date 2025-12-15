using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BallisticApp
{
    internal class TargetCanvasManager
    {
        private readonly Canvas canvas;
        private readonly int totalRings = 10;
        private AppSettings settings;
        private Shot shot;
        private BallisticCalculator calculator;

        private double targetCenterX;
        private double targetCenterY;
        private double padding = 20;

        public TargetCanvasManager(Canvas canvas, AppSettings settings, Shot shot, BallisticCalculator calculator)
        {
            this.canvas = canvas;
            this.settings = settings;
            this.shot = shot;
            this.calculator = calculator;

            targetCenterX = canvas.ActualWidth / 2;
            targetCenterY = canvas.ActualHeight / 2;

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
            canvas.Children.Clear();

            // Convert displacement to pixels (invert vertical!)
            double shotX = MetersToPixels(shot.horizontalDisplacement, settings.Ballistics.TargetRadius);
            double shotY = MetersToPixels(-shot.verticalDisplacement, settings.Ballistics.TargetRadius); // <-- invert

            // Adjust canvas and target center dynamically
            AdjustCanvasAndCenter(ref shotX, ref shotY);

            DrawCircle();
            AddMOATicks(Math.Max(Math.Abs(shotY), canvas.Height / 2));

            AddShot(targetCenterX + shotX, targetCenterY + shotY);
        }

        private void DrawAxesResult(Shot shot, BallisticCalculator calculator)
        {
            canvas.Children.Clear();

            double shotX = MetersToPixels(shot.horizontalDisplacement, settings.Ballistics.TargetRadius);
            double shotY = MetersToPixels(-shot.verticalDisplacement, settings.Ballistics.TargetRadius); // <-- invert

            AdjustCanvasAndCenter(ref shotX, ref shotY);

            // Draw axes
            var verticalLine = new Line
            {
                X1 = targetCenterX,
                Y1 = 0,
                X2 = targetCenterX,
                Y2 = canvas.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            var horizontalLine = new Line
            {
                X1 = 0,
                Y1 = targetCenterY,
                X2 = canvas.ActualWidth,
                Y2 = targetCenterY,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            canvas.Children.Add(verticalLine);
            canvas.Children.Add(horizontalLine);

            AddMOATicks(Math.Max(Math.Abs(shotY), canvas.Height / 2));

            AddShot(targetCenterX + shotX, targetCenterY + shotY);
        }

        private void AdjustCanvasAndCenter(ref double shotX, ref double shotY)
        {
            double shotPixelX = targetCenterX + shotX;
            double shotPixelY = targetCenterY + shotY;

            double deltaLeft = padding - shotPixelX;
            double deltaRight = shotPixelX - (canvas.Width - padding);
            double deltaTop = padding - shotPixelY;
            double deltaBottom = shotPixelY - (canvas.Height - padding);

            // Horizontal
            if (deltaLeft > 0)
            {
                canvas.Width += deltaLeft;
                targetCenterX += deltaLeft;
            }
            if (deltaRight > 0)
            {
                canvas.Width += deltaRight;
            }

            // Vertical
            if (deltaTop > 0)
            {
                canvas.Height += deltaTop;
                targetCenterY += deltaTop;
            }
            if (deltaBottom > 0)
            {
                canvas.Height += deltaBottom;
            }
        }

        private void DrawCircle()
        {
            double radiusIncrement = Math.Min(targetCenterX, targetCenterY) / totalRings;

            for (int i = totalRings; i >= 1; i--)
            {
                double diameter = radiusIncrement * i * 2;
                var ring = new Ellipse
                {
                    Width = diameter,
                    Height = diameter,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(ring, targetCenterX - diameter / 2);
                Canvas.SetTop(ring, targetCenterY - diameter / 2);
                canvas.Children.Add(ring);
            }

            for (int i = 1; i < totalRings; i++)
            {
                double d = radiusIncrement * (i + 0.5);
                int num = totalRings - i;
                AddNumber(num.ToString(), targetCenterX - d, targetCenterY);
                AddNumber(num.ToString(), targetCenterX + d, targetCenterY);
                AddNumber(num.ToString(), targetCenterX, targetCenterY - d);
                AddNumber(num.ToString(), targetCenterX, targetCenterY + d);
            }
            AddNumber("10", targetCenterX, targetCenterY);
        }

        private void AddMOATicks(double maxDistancePixels)
        {
            double tickIntervalMeters = calculator.ComputeMOADistance();
            for (double i = 0; i <= PixelsToMeters(maxDistancePixels, settings.Ballistics.TargetRadius); i += tickIntervalMeters)
            {
                double yPos = targetCenterY + MetersToPixels(-i, settings.Ballistics.TargetRadius); // invert
                var tick = new Line
                {
                    X1 = targetCenterX - 20,
                    Y1 = yPos,
                    X2 = targetCenterX + 20,
                    Y2 = yPos,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 1
                };
                canvas.Children.Add(tick);
            }
        }

        private void AddShot(double x, double y)
        {
            var hit = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(hit, x - hit.Width / 2);
            Canvas.SetTop(hit, y - hit.Height / 2);
            canvas.Children.Add(hit);
        }

        private void AddNumber(string text, double x, double y, double fontSize = 14)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                FontWeight = System.Windows.FontWeights.Bold
            };
            tb.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            var size = tb.DesiredSize;
            Canvas.SetLeft(tb, x - size.Width / 2);
            Canvas.SetTop(tb, y - size.Height / 2);
            canvas.Children.Add(tb);
        }

        public double MetersToPixels(double meters, double targetRadiusCm) =>
            meters * (canvas.ActualHeight / (targetRadiusCm * 2 / 100));

        public double PixelsToMeters(double pixels, double targetRadiusCm) =>
            pixels * ((targetRadiusCm * 2 / 100) / canvas.ActualHeight);
    }
}
