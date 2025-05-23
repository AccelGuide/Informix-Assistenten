﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Informix_Assistenten
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _testMode;
        private string _editorPath = "notepad.exe";

        public MainWindow(bool testMode)
        {
            InitializeComponent();
            lstMaintenanceTasks.Items.Clear();

            //Test:
            // MessageBox.Show($"Geladene Tasks: {MaintenanceManager.MaintenanceTasks.Count}");

            foreach (string name in MaintenanceManager.MaintenanceTasks)
            {
                lstMaintenanceTasks.Items.Add(new ListBoxItem { Content = name });
            }
            _testMode = testMode;
            LoadEditorPath();

            if (_testMode)
            {
                Title += " [Testmodus]";
                UpdateStatusLeiste("dummy", "dummy", "dummy");
            }
            else
            {
                Title += " [Verbunden]";
                UpdateStatusLeiste("ifxserver", "kunden_db", "admin"); // <- echte Werte hier einsetzen
            }
        }

        private void LoadEditorPath()
        {
            if (File.Exists("settings.config"))
            {
                string editorSetting = File.ReadLines("settings.config")
                                            .FirstOrDefault(line => line.StartsWith("EditorPath="));
                if (!string.IsNullOrWhiteSpace(editorSetting))
                {
                    _editorPath = editorSetting.Replace("EditorPath=", "").Trim();
                }
            }
        }
                private void UpdateStatusLeiste(string server, string datenbank, string benutzer)
        {
            txtStatusServer.Text = $"Server: {server}";
            txtStatusDatabase.Text = $"Datenbank: {datenbank}";
            txtStatusUser.Text = $"Benutzer: {benutzer}";
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuOpenManual_Click(object sender, RoutedEventArgs e)
        {
            string outputPath = Path.Combine(Path.GetTempPath(), "Manual.pdf");

            try
            {
                using (Stream resource = typeof(MainWindow).Assembly.GetManifestResourceStream("Informix_Assistenten.Manual.Manual.pdf"))
                {
                    if (resource == null)
                    {
                        MessageBox.Show("Eingebettetes Handbuch nicht gefunden.");
                        return;
                    }

                    using (FileStream file = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        resource.CopyTo(file);
                    }
                }

                Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Öffnen des Handbuchs: {ex.Message}");
            }
        }

        private void MenuNeu_Click(object sender, RoutedEventArgs e)
        {
            OpenEditorWithTempFile("neu.4gl");
        }

        private void MenuBearbeiten_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "4GL-Dateien (*.4gl;*.per)|*.4gl;*.per|Alle Dateien (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                OpenEditor(openFileDialog.FileName);
            }
        }

        private void MenuOeffnen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "4GL-Dateien (*.4gl;*.per)|*.4gl;*.per|Alle Dateien (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string content = File.ReadAllText(openFileDialog.FileName);
                txtOutput.Text = content;
            }
        }

        private void OpenEditorWithTempFile(string fileName)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllText(tempPath, "-- Neues Informix 4GL Programm");
            OpenEditor(tempPath);
        }

        private void OpenEditor(string filePath)
        {
            if (!string.IsNullOrEmpty(_editorPath) && File.Exists(_editorPath))
            {
                Process.Start(new ProcessStartInfo(_editorPath, filePath)
                {
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Kein gültiger Editorpfad konfiguriert.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuAnleitungHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Textdateien (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                string targetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Anleitungen");
                Directory.CreateDirectory(targetDir);
                string fileName = Path.GetFileName(openFileDialog.FileName);
                File.Copy(openFileDialog.FileName, Path.Combine(targetDir, fileName), true);
                MessageBox.Show("Anleitung hinzugefügt.");
            }
        }

        private void MenuAnleitungEntfernen_Click(object sender, RoutedEventArgs e)
        {
            string targetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Anleitungen");
            if (Directory.Exists(targetDir))
            {
                string[] files = Directory.GetFiles(targetDir, "*.txt");
                if (files.Length > 0)
                {
                    foreach (var file in files)
                        File.Delete(file);
                    MessageBox.Show("Alle Anleitungen entfernt.");
                }
                else
                {
                    MessageBox.Show("Keine Anleitungen gefunden.");
                }
            }
        }

        private void BtnExecuteProgramm_Click(object sender, RoutedEventArgs e)
        {
            if (_testMode)
                txtOutput.Text = "Testmodus aktiv: Simulierte Skriptausgabe.";
            else
                txtOutput.Text = "Standard-Skript ausgeführt."; // todo "Implementiren von funktionen"
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            lstRoutineTasks.Items.Add("Neue Aufgabe - " + DateTime.Now.ToShortTimeString());
        }

        private void BtnExecuteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstRoutineTasks.SelectedItem != null)
                txtRoutineTaskOutput.Text = $"Aufgabe '{lstRoutineTasks.SelectedItem}' ausgeführt.";
            else
                MessageBox.Show("Bitte wähle zuerst eine Aufgabe aus.");
        }

        private void BtnExecuteMaintenance_Click(object sender, RoutedEventArgs e)
        {
            if (lstMaintenanceTasks.SelectedItem != null)
            {
                string selectedTest = ((System.Windows.Controls.ListBoxItem)lstMaintenanceTasks.SelectedItem).Content.ToString();
                if (_testMode)
                    txtMaintenanceOutput.Text = $"[Testmodus] '{selectedTest}' simuliert ausgeführt. Ergebnisse hier.";
                else
                    txtMaintenanceOutput.Text = $"'{selectedTest}' wurde ausgeführt. Ergebnisse würden hier angezeigt werden.";
            }
            else
                MessageBox.Show("Bitte wähle zuerst einen Test aus.");
        }

        private bool ConnectToInformix(string server, string database, string user, string password)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Verbindungsfehler: {ex.Message}");
                return false;
            }
        }

        private void LogError(string message)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}\n");
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

        private void MenuEinstellungen_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }
        private void BtnNewLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Owner = this;
            loginWindow.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatusLeiste("dummy", "dummy", "dummy");
            Title = "Informix 4GL Assistent [Testmodus]";
            MessageBox.Show("Abgemeldet.");
        }
        private void lstMaintenanceTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstMaintenanceTasks.SelectedItem is ListBoxItem selectedItem)
            {
                string fileName = selectedItem.Content.ToString();
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maintenance_Prog");

                // .4gl zuerst probieren, dann .sh
                string[] extensions = new[] { ".4gl", ".sh" };
                foreach (string ext in extensions)
                {
                    string fullPath = Path.Combine(folder, fileName + ext);
                    if (File.Exists(fullPath))
                    {
                        OpenEditor(fullPath);
                        return;
                    }
                }

                MessageBox.Show("Datei nicht gefunden: " + fileName);
            }
        }
        private void lstExecuteProgramm_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstExecuteProgramm.SelectedItem is ListBoxItem selectedItem)
            {
                string fileName = selectedItem.Content.ToString();
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Execute_Prog");

                // .4gl zuerst probieren, dann .sh
                string[] extensions = new[] { ".4gl", ".sh" };
                foreach (string ext in extensions)
                {
                    string fullPath = Path.Combine(folder, fileName + ext);
                    if (File.Exists(fullPath))
                    {
                        OpenEditor(fullPath);
                        return;
                    }
                }

                MessageBox.Show("Datei nicht gefunden: " + fileName);
            }
        }
        private void lstRoutineTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstRoutineTasks.SelectedItem is ListBoxItem selectedItem)
            {
                string fileName = selectedItem.Content.ToString();
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RoutineTasks_Prog");

                // .4gl zuerst probieren, dann .sh
                string[] extensions = new[] { ".4gl", ".sh" };
                foreach (string ext in extensions)
                {
                    string fullPath = Path.Combine(folder, fileName + ext);
                    if (File.Exists(fullPath))
                    {
                        OpenEditor(fullPath);
                        return;
                    }
                }

                MessageBox.Show("Datei nicht gefunden: " + fileName);
            }
        }
    }
}
