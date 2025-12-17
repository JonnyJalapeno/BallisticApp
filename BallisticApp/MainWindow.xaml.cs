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
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;

            // Open settings window modal, with owner set
            settings = SettingsLoader.LoadSettings(this);
            if (settings == null)
            {
                Close();
                return;
            }


            shot = new Shot(settings.Ballistics);

            hud = new HUDManager(HudStackPanel, settings.Ballistics);
            hud.Render();

            targetManager = new TargetCanvasManager(TargetCanvas, settings, shot);
            //targetManager.DrawResult(settings.View.TargetType, shot);
            //targetManager.AddShot(calculator.ComputeVerticalDrop(), settings.Ballistics.TargetRadius);
            //targetManager.AddLine(settings.Ballistics.TargetRadius);
        }
    }
}
