using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallisticApp
{
    public class BallisticSettings
    {
        public double Distance { get; set; }
        public double ScopeHeight { get; set; }
        public double BallisticCoefficient { get; set; }

        public enum DragModelEnum
        {
            None,
            G1,
            G7
        }

        public DragModelEnum DragModel { get; set; } 
        public double TargetRadius { get; set; }
        public double BulletVelocity { get; set; }
        public double ZeroDistance { get; set; }
        public double WindDirection {get; set; }
        public double WindSpeed { get; set; }
    }
}
