using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallisticApp
{
    internal class Shot
    {
        public double verticalDisplacement { get; set; }
        public double horizontalDisplacement { get; set; }
        public double moaX { get; set; }
        public double moaY { get; set; }
        public double moa { get; set; }

        public Shot(BallisticSettings settings)
        {
            (double y, double x) = AdvancedBallisticsCalculatorImperial.CalculateImpact(settings);
            verticalDisplacement = y;
            horizontalDisplacement = x;
            moa = settings.Distance * Math.PI / 10800;
            moaX = horizontalDisplacement/moa;
            moaY = verticalDisplacement /moa;
        }

    }
}
