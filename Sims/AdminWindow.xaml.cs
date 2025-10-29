using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MyFirstWpfApp
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();

            // Bind the song list to DataStore
            SongsList.ItemsSource = DataStore.Songs;

            // 🧠 Load editors from a file (editors.csv)
            if (File.Exists("editors.csv"))
            {
                // Each line in editors.csv is one editor email
                EditorComboBox.ItemsSource = File.ReadAllLines("editors.csv");
            }
            else
            {
                // Fallback if file doesn’t exist yet
                EditorComboBox.ItemsSource = new string[]
                {
                    "editor@gmail.com",
                    "editor2@gmail.com",
                    "music.editor@gmail.com"
                };

                // Optional: create the file automatically
                File.WriteAllLines("editors.csv", new string[]
                {
                    "editor@gmail.com",
                    "editor2@gmail.com",
                    "music.editor@gmail.com"
                });
            }
        }

        private void SongsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongsList.SelectedItem is Song selectedSong)
            {
                // Load editors
                var editors = File.Exists("editors.csv")
                    ? File.ReadAllLines("editors.csv").ToList()
                    : new List<string> { "editor@gmail.com", "editor2@gmail.com", "music.editor@gmail.com" };

                editors.Insert(0, "None"); // <-- Add this at the top
                EditorComboBox.ItemsSource = editors;

                // Select the assigned editor or "None" if null/empty
                EditorComboBox.SelectedItem = string.IsNullOrEmpty(selectedSong.AssignedEditor)
                    ? "None"
                    : selectedSong.AssignedEditor;
            }
        }


        private void AssignSong_Click(object sender, RoutedEventArgs e)
        {
            var selectedSong = SongsList.SelectedItem as Song;
            if (selectedSong == null) return;

            var selectedEditor = EditorComboBox.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedEditor))
            {
                MessageBox.Show("Please select an editor from the dropdown.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // If "None" is selected, remove the editor assignment
            selectedSong.AssignedEditor = selectedEditor == "None" ? null : selectedEditor;

            DataStore.SaveSongs();

            MessageBox.Show($"'{selectedSong.Title}' is now assigned to '{selectedSong.AssignedEditor ?? "no one"}'.",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            DataStore.SaveSongs();
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
