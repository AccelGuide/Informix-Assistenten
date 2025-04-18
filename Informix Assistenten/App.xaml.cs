using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Informix_Assistenten
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SplashWindow splash = new SplashWindow();
            splash.Show();
        }

    }
    public static class MaintenanceManager
    {
        public static List<string> MaintenanceTasks { get; private set; } = new List<string>();

        public static void LoadTasks()
        {
            MaintenanceTasks.Clear();

            string progFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maintenance_Prog");
            if (!Directory.Exists(progFolder))
                Directory.CreateDirectory(progFolder);

            var shFiles = Directory.GetFiles(progFolder, "*.sh");
            var glFiles = Directory.GetFiles(progFolder, "*.4gl");

            foreach (var file in glFiles.Concat(shFiles))
            {
                MaintenanceTasks.Add(Path.GetFileNameWithoutExtension(file));
            }
        }
    }
}
