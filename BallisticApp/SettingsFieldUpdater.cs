/*using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallisticApp
{
    internal class SettingsFieldUpdater
    {
        public static void Apply(SettingsWindow w, AppSettings settings)
        {
            // Use CultureInfo.InvariantCulture to ensure consistent formatting
            var culture = CultureInfo.InvariantCulture;

            // Ammunition+Firearm tab
            DefaultTextBehavior.SetCurrentText(w.BallisticCoefficientTextBox,
                settings.Ballistics.BallisticCoefficient.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.BulletWeightTextBox,
                settings.Ballistics.BulletWeight.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.BulletDiameterTextBox,
                settings.Ballistics.BulletDiameter.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.MuzzleVelocityTextBox,
                settings.Ballistics.BulletVelocity.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.BarrelTwistTextBox,
                settings.Ballistics.BarrelTwist.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.SightHeightTextBox,
                settings.Ballistics.SightHeight.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.ZeroDistanceTextBox,
                settings.Ballistics.ZeroDistance.ToString(culture));

            // Conditions+Environment tab
            DefaultTextBehavior.SetCurrentText(w.WindSpeedTextBox,
                settings.Ballistics.WindSpeed.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.WindDirectionTextBox,
                settings.Ballistics.WindDirection.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.AltitudeTextBox,
                settings.Ballistics.Altitude.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.PressureTextBox,
                settings.Ballistics.Pressure.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.TemperatureTextBox,
                settings.Ballistics.Temperature.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.HumidityTextBox,
                settings.Ballistics.Humidity.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.ShootingAngleTextBox,
                settings.Ballistics.ShootingAngle.ToString(culture));

            // Output settings tab
            DefaultTextBehavior.SetCurrentText(w.DistanceTextBox,
                settings.Ballistics.Distance.ToString(culture));
            DefaultTextBehavior.SetCurrentText(w.TargetRadiusTextBox,
                settings.Ballistics.TargetRadius.ToString(culture));
        }

    }
}
*/
