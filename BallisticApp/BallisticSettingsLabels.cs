using System.Collections.Generic;

namespace BallisticApp
{
    public static class BallisticSettingsLabels
    {
        public static readonly Dictionary<string, string> Labels = new()
        {
            { nameof(BallisticSettings.Distance), "Distance (m)" },
            { nameof(BallisticSettings.ScopeHeight), "Scope Height (cm)" },
            { nameof(BallisticSettings.BallisticCoefficient), "Ballistic Coefficient" },
            { nameof(BallisticSettings.DragModel), "Drag Model" },
            { nameof(BallisticSettings.TargetRadius), "Target Radius (cm)" },
            { nameof(BallisticSettings.BulletVelocity), "Bullet Velocity (m/s)" },
            { nameof(BallisticSettings.ZeroDistance), "Zero Distance (m)" },
            { nameof(BallisticSettings.WindDirection), "Wind Direction (deg)" },
            { nameof(BallisticSettings.WindSpeed), "Wind Speed (m/s)" }
        };
    }
}
