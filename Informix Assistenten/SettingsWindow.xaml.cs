using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Informix_Assistenten
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly string configPath = "settings.config";

        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (File.Exists(configPath))
            {
                var lines = File.ReadAllLines(configPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("Testmode=true")) chkTestmode.IsChecked = true;
                    if (line.StartsWith("LogAutoSave=true")) chkLogAutoSave.IsChecked = true;
                    if (line.StartsWith("EditorPath="))
                    {
                        string path = line.Replace("EditorPath=", "");
                        txtEditorPath.Text = path;

                        if (path.EndsWith("notepad.exe", StringComparison.OrdinalIgnoreCase))
                            cmbEditor.SelectedIndex = 0;
                        else if (path.IndexOf("notepad++", StringComparison.OrdinalIgnoreCase) >= 0)
                            cmbEditor.SelectedIndex = 1;
                        else if (path.IndexOf("Code.exe", StringComparison.OrdinalIgnoreCase) >= 0)
                            cmbEditor.SelectedIndex = 2;
                        else
                            cmbEditor.SelectedIndex = 3;
                    }
                }
            }
        }

        private void SaveSettings()
        {
            string[] lines = new string[]
            {
                $"Testmode={(chkTestmode.IsChecked == true).ToString().ToLower()}",
                $"LogAutoSave={(chkLogAutoSave.IsChecked == true).ToString().ToLower()}",
                $"EditorPath={txtEditorPath.Text}"
            };
            File.WriteAllLines(configPath, lines);
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEditor.SelectedItem is ComboBoxItem selected && selected.Content.ToString() == "Andere")
            {
                if (string.IsNullOrWhiteSpace(txtEditorPath.Text) || !File.Exists(txtEditorPath.Text))
                {
                    MessageBox.Show("Bitte geben Sie einen gültigen Programmpfad ein.", "Ungültiger Pfad", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            SaveSettings();
            this.Close();
        }

        private void BtnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void cmbEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtEditorPath == null || cmbEditor.SelectedItem == null)
                return;

            if (cmbEditor.SelectedItem is ComboBoxItem selectedItem)
            {
                string selected = selectedItem.Content.ToString();

                switch (selected)
                {
                    case "Notepad":
                        txtEditorPath.Text = "notepad.exe";
                        txtEditorPath.IsReadOnly = true;
                        break;

                    case "Notepad++":
                        txtEditorPath.Text = @"C:\Program Files\Notepad++\notepad++.exe";
                        txtEditorPath.IsReadOnly = true;
                        break;

                    case "Visual Studio Code":
                        string vscodePath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"Programs\Microsoft VS Code\Code.exe"
                        );

                        if (File.Exists(vscodePath))
                        {
                            txtEditorPath.Text = vscodePath;
                        }
                        else
                        {
                            txtEditorPath.Text = ""; // Falls nicht gefunden
                            MessageBox.Show("Visual Studio Code konnte nicht im Standardverzeichnis gefunden werden.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                        txtEditorPath.IsReadOnly = true;
                        break;

                    case "Andere":
                        txtEditorPath.Text = "";
                        txtEditorPath.IsReadOnly = false;
                        break;
                }
            }
        }

        private void BtnChoosePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "4GL-Dateien (*.4gl;*.per)|*.4gl;*.per|Alle Dateien (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                txtEditorPath.Text = openFileDialog.FileName;
            }
        }
    }
}
