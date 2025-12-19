namespace BallisticApp
{
    public record DefaultValues
    {
        // Ammunition + Firearm
        public double BallisticCoefficient { get; init; } = 0.415;
        public double BulletWeight { get; init; } = 150;
        public double BulletDiameter { get; init; } = 7.82;
        public double BulletVelocity { get; init; } = 900;
        public double BarrelTwist { get; init; } = 177.8;
        public double SightHeight { get; init; } = 5;
        public double ZeroDistance { get; init; } = 100;
        public double Distance { get; init; } = 400;
        public double TargetRadius { get; init; } = 50;

        // Conditions + Environment
        public double WindSpeed { get; init; } = 5;
        public double WindDirection { get; init; } = 90;
        public double Altitude { get; init; } = 200;
        public double Pressure { get; init; } = 1013.2;
        public double Temperature { get; init; } = 15;
        public double Humidity { get; init; } = 50;
        public double ShootingAngle { get; init; } = 0;
    }
}