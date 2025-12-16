using System;

namespace BallisticApp
{
    public class BallisticSettings
    {
        // Ammunition + Firearm
        public double BallisticCoefficient { get; set; }
        public double BulletWeight { get; set; }
        public double BulletDiameter { get; set; }
        public double BulletVelocity { get; set; }
        public double BarrelTwist { get; set; }
        public double SightHeight { get; set; }
        public double ZeroDistance { get; set; }
        public double Distance { get; set; }
        public double TargetRadius { get; set; }

        // Conditions + Environment
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public double Altitude { get; set; }
        public double Pressure { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double ShootingAngle { get; set; }

        public enum DragModelEnum
        {
            None,
            G1,
            G7
        }

        public DragModelEnum DragModel { get; set; }
    }
}