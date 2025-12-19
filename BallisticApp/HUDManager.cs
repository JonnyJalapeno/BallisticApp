using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using BallisticApp;

namespace BallisticApp
{
    internal class HUDManager
    {
        private  StackPanel container;
        private  BallisticSettings settings;

        public HUDManager(StackPanel container, BallisticSettings settings)
        {
            this.container = container;
            this.settings = settings;
        }

        public void Update(AppSettings settings)       
        {
            this.settings = settings.Ballistics;

        }

        public void Render()
        {
            container.Children.Clear();
            PropertyInfo[] props = settings.GetType().GetProperties();

            foreach (var prop in typeof(BallisticSettings).GetProperties())
            {
                string key = prop.Name; // e.g., "Distance"
                string label = BallisticSettingsLabels.Labels[key]; // "Distance (m)"
                object value = prop.GetValue(settings); // the actual value, e.g., 300

                container.Children.Add(new TextBlock
                {
                    Text = $"{label}: {value}"
                });
            }
        }
    }
}
