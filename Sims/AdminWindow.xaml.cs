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

                editors.Insert(0, "None");
                EditorComboBox.ItemsSource = editors;

                EditorComboBox.SelectedItem = string.IsNullOrEmpty(selectedSong.AssignedEditor)
                    ? "None"
                    : selectedSong.AssignedEditor;

                // Display grade and comment
                SelectedGrade.Text = selectedSong.Grade.HasValue ? $"Grade: {selectedSong.Grade}" : "Grade: N/A";
                SelectedComment.Text = !string.IsNullOrEmpty(selectedSong.Review) ? $"Comment: {selectedSong.Review}" : "Comment: N/A";

                // Disable Assign button and ComboBox if reviewed
                bool isReviewed = selectedSong.Grade.HasValue;
                AssignButton.IsEnabled = !isReviewed;
                EditorComboBox.IsEnabled = !isReviewed;
            }
            else
            {
                SelectedGrade.Text = "";
                SelectedComment.Text = "";
                AssignButton.IsEnabled = true;
                EditorComboBox.IsEnabled = true;
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

            // Assign or remove editor
            selectedSong.AssignedEditor = selectedEditor == "None" ? null : selectedEditor;

            // Save changes
            DataStore.SaveSongs();

            // Refresh the ListBox so opacity updates immediately
            SongsList.Items.Refresh();

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
