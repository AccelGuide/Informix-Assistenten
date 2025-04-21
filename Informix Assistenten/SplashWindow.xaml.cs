using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                "Starte Anwendung..."
            };

            for (int i = 0; i < ladeText.Length; i++)
            {
                txtStatus.Text = ladeText[i];
                progressBar.Value = (i + 1) * (100 / ladeText.Length);
                await Task.Delay(700);
            }

            MaintenanceManager.LoadTasks();

            var ports = LoadPortsFromConfig();
            txtStatus.Text = "Scanne nach verfügbaren Informix-Servern...";
            AppState.AvailableServers = await InformixServerScanner.ScanAsync(ports, msg =>
            {
                Dispatcher.Invoke(() => txtStatus.Text = "Gefunden: " + msg);
            });

            MainWindow mainWindow = new MainWindow(testMode: true);
            mainWindow.Show();
            this.Close();
        }

        private List<int> LoadPortsFromConfig()
        {
            var result = new List<int>();
            string configPath = "settings.config";

            if (File.Exists(configPath))
            {
                var lines = File.ReadAllLines(configPath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("Ports="))
                    {
                        var portStrings = line.Replace("Ports=", "").Split(',');
                        foreach (var p in portStrings)
                        {
                            if (int.TryParse(p.Trim(), out int port))
                                result.Add(port);
                        }
                    }
                }
            }

            if (result.Count == 0)
                result.Add(1526); // Fallback-Port

            return result;
        }
    }
}
