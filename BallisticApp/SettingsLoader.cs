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
    }
}
