using System.Windows;

namespace BallisticApp
{
    public partial class MainWindow : Window
    {
        private AppSettings settings;
        private BallisticCalculator calculator;
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

            calculator = new BallisticCalculator(settings.Ballistics);
            //shot = new Shot(calculator);
            shot = new Shot(settings.Ballistics);

            hud = new HUDManager(HudStackPanel, settings.Ballistics);
            hud.Render();

            targetManager = new TargetCanvasManager(TargetCanvas, settings, shot, calculator);
            //targetManager.DrawResult(settings.View.TargetType, shot);
            //targetManager.AddShot(calculator.ComputeVerticalDrop(), settings.Ballistics.TargetRadius);
            //targetManager.AddLine(settings.Ballistics.TargetRadius);
        }
    }
}
