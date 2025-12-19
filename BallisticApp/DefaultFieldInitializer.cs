using System.Globalization;
using System.Windows.Controls;

namespace BallisticApp
{
    public static class DefaultFieldInitializer
    {
        private static readonly DefaultValues _defaults = new();

        public static void Apply(SettingsWindow w, AppSettings? settings = null)
        {
            var culture = CultureInfo.InvariantCulture;

            // Helper to assign default and current value
            void Init(TextBox tb, double defaultVal, double? currentVal = null)
            {
                string defStr = defaultVal.ToString(culture);
                string curStr = (currentVal ?? defaultVal).ToString(culture);

                DefaultTextBehavior.SetDefaultText(tb, defStr);
                DefaultTextBehavior.SetCurrentText(tb, curStr);
            }

            // Ammunition+Firearm tab
            Init(w.BallisticCoefficientTextBox, _defaults.BallisticCoefficient, settings?.Ballistics.BallisticCoefficient);
            Init(w.BulletWeightTextBox, _defaults.BulletWeight, settings?.Ballistics.BulletWeight);
            Init(w.BulletDiameterTextBox, _defaults.BulletDiameter, settings?.Ballistics.BulletDiameter);
            Init(w.MuzzleVelocityTextBox, _defaults.BulletVelocity, settings?.Ballistics.BulletVelocity);
            Init(w.BarrelTwistTextBox, _defaults.BarrelTwist, settings?.Ballistics.BarrelTwist);
            Init(w.SightHeightTextBox, _defaults.SightHeight, settings?.Ballistics.SightHeight);
            Init(w.ZeroDistanceTextBox, _defaults.ZeroDistance, settings?.Ballistics.ZeroDistance);

            // Conditions+Environment tab
            Init(w.WindSpeedTextBox, _defaults.WindSpeed, settings?.Ballistics.WindSpeed);
            Init(w.WindDirectionTextBox, _defaults.WindDirection, settings?.Ballistics.WindDirection);
            Init(w.AltitudeTextBox, _defaults.Altitude, settings?.Ballistics.Altitude);
            Init(w.PressureTextBox, _defaults.Pressure, settings?.Ballistics.Pressure);
            Init(w.TemperatureTextBox, _defaults.Temperature, settings?.Ballistics.Temperature);
            Init(w.HumidityTextBox, _defaults.Humidity, settings?.Ballistics.Humidity);
            Init(w.ShootingAngleTextBox, _defaults.ShootingAngle, settings?.Ballistics.ShootingAngle);

            // Output tab
            Init(w.DistanceTextBox, _defaults.Distance, settings?.Ballistics.Distance);
            Init(w.TargetRadiusTextBox, _defaults.TargetRadius, settings?.Ballistics.TargetRadius);
        }

        public static void ApplyDefaults(SettingsWindow w)
        {
            var culture = CultureInfo.InvariantCulture;

            // Ammunition+Firearm tab
            SetDefaultAndCurrent(w.BallisticCoefficientTextBox, _defaults.BallisticCoefficient, culture);
            SetDefaultAndCurrent(w.BulletWeightTextBox, _defaults.BulletWeight, culture);
            SetDefaultAndCurrent(w.BulletDiameterTextBox, _defaults.BulletDiameter, culture);
            SetDefaultAndCurrent(w.MuzzleVelocityTextBox, _defaults.BulletVelocity, culture);
            SetDefaultAndCurrent(w.BarrelTwistTextBox, _defaults.BarrelTwist, culture);
            SetDefaultAndCurrent(w.SightHeightTextBox, _defaults.SightHeight, culture);
            SetDefaultAndCurrent(w.ZeroDistanceTextBox, _defaults.ZeroDistance, culture);

            // Conditions+Environment tab
            SetDefaultAndCurrent(w.WindSpeedTextBox, _defaults.WindSpeed, culture);
            SetDefaultAndCurrent(w.WindDirectionTextBox, _defaults.WindDirection, culture);
            SetDefaultAndCurrent(w.AltitudeTextBox, _defaults.Altitude, culture);
            SetDefaultAndCurrent(w.PressureTextBox, _defaults.Pressure, culture);
            SetDefaultAndCurrent(w.TemperatureTextBox, _defaults.Temperature, culture);
            SetDefaultAndCurrent(w.HumidityTextBox, _defaults.Humidity, culture);
            SetDefaultAndCurrent(w.ShootingAngleTextBox, _defaults.ShootingAngle, culture);

            // Output settings tab
            SetDefaultAndCurrent(w.DistanceTextBox, _defaults.Distance, culture);
            SetDefaultAndCurrent(w.TargetRadiusTextBox, _defaults.TargetRadius, culture);
        }

        private static void SetDefaultAndCurrent(TextBox tb, double value, CultureInfo culture)
        {
            string strValue = value.ToString(culture);
            DefaultTextBehavior.SetDefaultText(tb, strValue);
            DefaultTextBehavior.SetCurrentText(tb, strValue);
        }
    }
}
