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

        /*public Shot(BallisticCalculator calculator) {
            verticalDrop = calculator.ComputeVerticalDrop();
        }*/
        public Shot(BallisticSettings settings)
        {
            (double y, double x) = AdvancedBallisticsCalculatorImperial.CalculateImpact(settings);
            verticalDisplacement = y;
            horizontalDisplacement = x;
        }

    }
}
