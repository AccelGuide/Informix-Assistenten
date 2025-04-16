using System.Windows;

namespace Informix_Assistenten
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectToInformix(txtServer.Text, txtDatabase.Text, txtUser.Text, txtPassword.Password))
            {
                OpenMainWindow(false);
            }
            else
            {
                MessageBox.Show("Verbindung fehlgeschlagen! Bitte überprüfe die Eingaben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                LogError("Verbindung fehlgeschlagen beim Versuch, mit echten Zugangsdaten zu verbinden.");
            }
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            OpenMainWindow(true);
        }

        private bool ConnectToInformix(string server, string database, string user, string password)
        {
            try
            {


                // Temporäre Dummy-Logik für Demo-Zwecke:
                return !string.IsNullOrWhiteSpace(server) &&
                       !string.IsNullOrWhiteSpace(database) &&
                       !string.IsNullOrWhiteSpace(user) &&
                       !string.IsNullOrWhiteSpace(password);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Verbindungsfehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                LogError($"Exception bei Verbindungsversuch: {ex.Message}");
                return false;
            }
        }

        private void OpenMainWindow(bool testMode)
        {
            MainWindow main = new MainWindow(testMode);
            main.Show();
            Close();
        }

        private void LogError(string message)
        {
            string logPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "error.log");
            System.IO.File.AppendAllText(logPath, $"{System.DateTime.Now}: {message}\n");
        }
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

    }
}
