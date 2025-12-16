using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BallisticApp
{
    public static class DefaultTextBehavior
    {
        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.RegisterAttached(
                "DefaultText",
                typeof(string),
                typeof(DefaultTextBehavior),
                new PropertyMetadata(string.Empty, OnDefaultTextChanged));

        public static string GetDefaultText(TextBox textBox)
            => (string)textBox.GetValue(DefaultTextProperty);

        public static void SetDefaultText(TextBox textBox, string value)
            => textBox.SetValue(DefaultTextProperty, value);

        private static void OnDefaultTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb) return;

            string defaultValue = (string)e.NewValue;

            Brush normalBg = tb.Background;
            Brush defaultBg = new SolidColorBrush(Color.FromRgb(235, 235, 235));

            void ApplyDefault()
            {
                tb.Text = defaultValue;
                tb.Background = defaultBg;
            }

            void ApplyNormal()
            {
                tb.Background = normalBg;
            }

            ApplyDefault();

            tb.GotFocus += (_, _) =>
            {
                if (tb.Text == defaultValue)
                {
                    tb.Text = "";
                    ApplyNormal();
                }
            };

            tb.LostFocus += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    ApplyDefault();
                }
            };
        }
    }
}
