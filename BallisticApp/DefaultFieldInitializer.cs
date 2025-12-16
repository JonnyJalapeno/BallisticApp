namespace BallisticApp
{
    public static class DefaultFieldInitializer
    {
        public static void Apply(SettingsWindow w)
        {
            // Ammunition+Firearm tab
            DefaultTextBehavior.SetDefaultText(w.BallisticCoefficientTextBox, "0.415");
            DefaultTextBehavior.SetDefaultText(w.BulletWeightTextBox, "150");
            DefaultTextBehavior.SetDefaultText(w.BulletDiameterTextBox, "7.82");
            DefaultTextBehavior.SetDefaultText(w.MuzzleVelocityTextBox, "900");
            DefaultTextBehavior.SetDefaultText(w.BarrelTwistTextBox, "177.8");
            DefaultTextBehavior.SetDefaultText(w.SightHeightTextBox, "5");
            DefaultTextBehavior.SetDefaultText(w.ZeroDistanceTextBox, "100");
            // Conditions+Environment tab
            DefaultTextBehavior.SetDefaultText(w.WindSpeedTextBox, "0");
            DefaultTextBehavior.SetDefaultText(w.WindDirectionTextBox, "0");
            DefaultTextBehavior.SetDefaultText(w.AltitudeTextBox, "200");
            DefaultTextBehavior.SetDefaultText(w.PressureTextBox, "1013.2");
            DefaultTextBehavior.SetDefaultText(w.TemperatureTextBox, "15");
            DefaultTextBehavior.SetDefaultText(w.HumidityTextBox, "50");
            DefaultTextBehavior.SetDefaultText(w.ShootingAngleTextBox, "0");
            // Output settings tab
            DefaultTextBehavior.SetDefaultText(w.DistanceTextBox, "400");
            DefaultTextBehavior.SetDefaultText(w.TargetRadiusTextBox, "50");
        }
    }
}