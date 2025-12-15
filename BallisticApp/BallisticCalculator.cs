using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BallisticApp
{
    internal class BallisticCalculator
    {
        private readonly BallisticSettings settings;

        public BallisticCalculator(BallisticSettings settings)
        {
            this.settings = settings;
        }

        public double ComputeVerticalDrop()
        {
            const double g = 9.81;
            double time = settings.Distance / settings.BulletVelocity;
            return 0.5 * g * time * time;
        }

        public double ComputeMOADistance()
        {
            double angleRad = (1 / 60.0) * (Math.PI / 180.0);
            return settings.Distance * Math.Tan(angleRad);
        }

        
    }
}
