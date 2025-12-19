using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BallisticApp
{
    internal class ShotInfoPanel
    {
        public void AddShotInfoPanel(Canvas canvas, Shot shot, double x, double y)
        {
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Background = System.Windows.Media.Brushes.LightGray,
                Opacity = 0.8,
                
            };
            Canvas.SetTop(panel, y+10);
            Canvas.SetLeft(panel, x-40);
            string labelH = "Windage";
            string labelV = "Elevation";
            double moaX = Math.Floor(shot.moaX*100)/100;
            double moaY = Math.Floor(shot.moaY*100)/100;
            panel.Children.Add(new TextBlock { Text = $"{labelH}: {moaX}" });
            panel.Children.Add(new TextBlock { Text = $"{labelV}: {moaY}" });
            Canvas.SetZIndex(panel, 10);
            canvas.Children.Add(panel);
        }
    }
}
