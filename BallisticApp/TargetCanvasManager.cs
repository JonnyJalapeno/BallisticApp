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

        private double targetCenterX;
        private double targetCenterY;
        private double padding = 20;

        public TargetCanvasManager(Canvas canvas, AppSettings settings, Shot shot)
        {
            this.canvas = canvas;
            this.settings = settings;
            this.shot = shot; 

            DrawResult(settings.View.TargetType, this.shot);
        }

        public void DrawResult(ViewSettings.TargetKind targetType, Shot shot)
        {
            switch (targetType)
            {
                case ViewSettings.TargetKind.Circular:
                    DrawCircularResult(shot);
                    break;
                case ViewSettings.TargetKind.Axes:
                    DrawAxesResult(shot);
                    break;
            }
        }

        private void DrawCircularResult(Shot shot)
        {
            ResetCanvas();

            // Convert displacement to pixels (invert vertical!)
            double shotX = MetersToPixels(shot.horizontalDisplacement, settings.Ballistics.TargetRadius);
            double shotY = MetersToPixels(-shot.verticalDisplacement, settings.Ballistics.TargetRadius); // <-- invert

            // Adjust canvas and target center dynamically
            AdjustCanvasAndCenter(ref shotX, ref shotY);

            DrawCircle();

            AddShot(targetCenterX + shotX, targetCenterY + shotY);
            AddMOATicks(shotX, shotY, shot);
        }

        private void DrawAxesResult(Shot shot)
        {
            ResetCanvas();

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

            AddShot(targetCenterX + shotX, targetCenterY + shotY);
            AddMOATicks(shotX, shotY, shot);
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

        private void AddMOATicks(double shotX, double shotY, Shot shot)
        {
            double tickIntervalPixels = MetersToPixels(shot.moa, settings.Ballistics.TargetRadius);
            double maxY = Math.Ceiling(shotY / tickIntervalPixels)*tickIntervalPixels;
            if ((maxY + targetCenterY) > canvas.Height) {
                canvas.Height = maxY + targetCenterY + padding;
            }
            double maxX = Math.Ceiling(shotX / tickIntervalPixels) * tickIntervalPixels;
            if ((maxX + targetCenterX) > canvas.Width)
            {
                canvas.Width = maxX + targetCenterX + padding;
            }
            //vertical
            for (double i = tickIntervalPixels; i <= maxY; i += tickIntervalPixels)
            {
                var tick = new Line
                {
                    X1 = targetCenterX + shotX - 20,
                    Y1 = targetCenterY + i,
                    X2 = targetCenterX + shotX + 20,
                    Y2 = targetCenterY + i,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 2
                };
                canvas.Children.Add(tick);
            }

            //horizontal
            for (double i = tickIntervalPixels; i <= maxX; i += tickIntervalPixels)
            {
                var tick = new Line
                {
                    X1 = targetCenterX + i,
                    Y1 = targetCenterY + shotY - 20,
                    X2 = targetCenterX + i,
                    Y2 = targetCenterY + shotY + 20,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 2
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
            ShotInfoPanel shotInfoPanel = new ShotInfoPanel();
            shotInfoPanel.AddShotInfoPanel(canvas,shot, x,y);
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

        private void ResetCanvas()
        {
            canvas.Children.Clear();
            canvas.Width = 600;
            canvas.Height = 600;
            canvas.UpdateLayout();

            targetCenterX = canvas.ActualWidth / 2;
            targetCenterY = canvas.ActualHeight / 2;
        }
    }
}
