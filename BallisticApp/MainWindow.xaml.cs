using System.Windows;

namespace BallisticApp
{
    public partial class MainWindow : Window
    {
        private AppSettings settings;
        private Shot shot;
        private HUDManager hud;
        private TargetCanvasManager targetManager;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            ReturnButton.Click += ReturnButton_Click;     
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;

            settings = SettingsLoader.LoadSettings(this);
            if (settings == null)
            {
                Close();
                return;
            }

            shot = new Shot(settings.Ballistics);
            hud = new HUDManager(HudStackPanel, settings.Ballistics);
            hud.Render();
            HudOverlay.Visibility = Visibility.Visible;
            targetManager = new TargetCanvasManager(TargetCanvas, settings, shot);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.settings = SettingsLoader.UpdateSettings(this, this.settings);
            if (settings == null)
            {
                Close();
                return;
            }
            hud.Update(settings);
            hud.Render();
            shot = new Shot(settings.Ballistics);

            targetManager = new TargetCanvasManager(TargetCanvas, settings, shot);
        }
    }
}
