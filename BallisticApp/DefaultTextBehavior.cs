using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BallisticApp
{
    public static class DefaultTextBehavior
    {
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.RegisterAttached(
                "CurrentValue",
                typeof(string),
                typeof(DefaultTextBehavior),
                new PropertyMetadata(string.Empty, OnValueChanged));

        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.RegisterAttached(
                "DefaultValue",
                typeof(string),
                typeof(DefaultTextBehavior));

        public static string GetDefaultText(TextBox textBox) => (string)textBox.GetValue(DefaultValueProperty);
        public static void SetDefaultText(TextBox textBox, string value) => textBox.SetValue(DefaultValueProperty, value);

        public static string GetCurrentText(TextBox textBox) => (string)textBox.GetValue(CurrentValueProperty);
        public static void SetCurrentText(TextBox textBox, string value) => textBox.SetValue(CurrentValueProperty, value);

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb) return;

            string defaultValue = GetDefaultText(tb);
            string currentValue = GetCurrentText(tb);

            Brush normalBg = Brushes.White;
            Brush defaultBg = new SolidColorBrush(Color.FromRgb(235, 235, 235));

            void SyncBackground()
            {
                tb.Background = (tb.Text == defaultValue) ? defaultBg : normalBg;
            }

            // Initial sync
            if (string.IsNullOrEmpty(tb.Text))
            {
                tb.Text = currentValue ?? defaultValue;
            }
            SyncBackground();

            tb.GotFocus += (_, _) =>
            {
                if (tb.Text == defaultValue)
                {
                    tb.Text = "";
                    tb.Background = normalBg;
                }
            };

            tb.LostFocus += (_, _) =>
            {
                currentValue = tb.Text;
                SetCurrentText(tb, currentValue);

                // Only change background if value matches default
                SyncBackground();
            };
        }
    }
}
