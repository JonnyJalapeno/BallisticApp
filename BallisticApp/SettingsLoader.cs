using System.Windows;

namespace BallisticApp
{
    internal static class SettingsLoader
    {
        public static AppSettings LoadSettings(Window owner)
        {
            var sw = new SettingsWindow
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (sw.ShowDialog() != true)
                return null;

            return new AppSettings
            {
                Ballistics = sw.Ballistics,
                View = sw.View
            };
        }

        public static AppSettings UpdateSettings(Window owner, AppSettings settings) 
        {
            var sw = new SettingsWindow()
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (sw.ShowDialog() != true)
                return null;

            settings.Ballistics = sw.Ballistics;
            settings.View = sw.View;

            return settings;
        }   
    }
}
