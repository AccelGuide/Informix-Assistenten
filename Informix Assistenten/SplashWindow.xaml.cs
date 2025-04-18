using System.Threading.Tasks;
using System.Windows;

namespace Informix_Assistenten
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            Loaded += SplashWindow_Loaded;
        }

        private async void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string[] ladeText = new[]
            {
                "Lade Hauptprogrammdaten...",
                "Lade Konfigurationsdaten...",
                "Prüfe Datenbankverbindung...",
                "Initialisiere Oberfläche...",
                "Starte Anwendung im Testmodus..."
            };

            for (int i = 0; i < ladeText.Length; i++)
            {
                txtStatus.Text = ladeText[i];
                progressBar.Value = (i + 1) * (100 / ladeText.Length);
                await Task.Delay(700); // Simulierter Ladeschritt
            }

            MaintenanceManager.LoadTasks();
            // Splash schließen und Hauptfenster starten
            MainWindow mainWindow = new MainWindow(testMode: true); // true für Testmodus
            mainWindow.Show();
            this.Close();
        }
    }
}
