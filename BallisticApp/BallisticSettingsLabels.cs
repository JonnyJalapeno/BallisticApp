using System.Collections.Generic;

namespace BallisticApp
{
    public static class BallisticSettingsLabels
    {
        public static readonly Dictionary<string, string> Labels = new()
        {
            // Ammunition + Firearm
            { nameof(BallisticSettings.BallisticCoefficient), "Ballistic Coefficient" },
            { nameof(BallisticSettings.BulletWeight), "Bullet Weight (gr)" },
            { nameof(BallisticSettings.BulletDiameter), "Bullet Diameter (mm)" },
            { nameof(BallisticSettings.BulletVelocity), "Muzzle Velocity (m/s)" },
            { nameof(BallisticSettings.BarrelTwist), "Barrel Twist (mm/rev)" },
            { nameof(BallisticSettings.SightHeight), "Scope Height (cm)" },
            { nameof(BallisticSettings.ZeroDistance), "Zero Distance (m)" },
            { nameof(BallisticSettings.Distance), "Distance to Target (m)" },
            { nameof(BallisticSettings.TargetRadius), "Target Radius (cm)" },

            // Conditions + Environment
            { nameof(BallisticSettings.WindSpeed), "Wind Speed (m/s)" },
            { nameof(BallisticSettings.WindDirection), "Wind Direction (deg)" },
            { nameof(BallisticSettings.Altitude), "Altitude (m)" },
            { nameof(BallisticSettings.Pressure), "Pressure (hPa)" },
            { nameof(BallisticSettings.Temperature), "Temperature (°C)" },
            { nameof(BallisticSettings.Humidity), "Humidity (%)" },
            { nameof(BallisticSettings.ShootingAngle), "Shooting Angle (deg)" },

            // Drag Model
            { nameof(BallisticSettings.DragModel), "Drag Model" }
        };
    }
}
