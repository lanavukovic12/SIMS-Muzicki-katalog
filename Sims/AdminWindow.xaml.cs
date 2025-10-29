using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MyFirstWpfApp
{

    public partial class AdminWindow : Window
    {
        private bool editorsLoaded = false;
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
            var selectedSongs = SongsList.SelectedItems.Cast<Song>().ToList();

            if (!editorsLoaded)
            {
                // Load editors once
                var editors = File.Exists("editors.csv")
                    ? File.ReadAllLines("editors.csv").ToList()
                    : new List<string> { "editor@gmail.com", "editor2@gmail.com", "music.editor@gmail.com" };

                editors.Insert(0, "None"); // Add "None" at the top
                EditorComboBox.ItemsSource = editors;
                editorsLoaded = true;
            }

            if (selectedSongs.Any())
            {
                // Show grade/comment for first selected song only
                var firstSong = selectedSongs.First();
                SelectedGrade.Text = firstSong.Grade.HasValue ? $"Grade: {firstSong.Grade}" : "Grade: N/A";
                SelectedComment.Text = !string.IsNullOrEmpty(firstSong.Review) ? $"Comment: {firstSong.Review}" : "Comment: N/A";

                // Disable assign if any song is reviewed
                bool anyReviewed = selectedSongs.Any(s => s.Grade.HasValue || !string.IsNullOrEmpty(s.Review));
                AssignButton.IsEnabled = !anyReviewed;
                EditorComboBox.IsEnabled = !anyReviewed;

                // Set selected editor only if exactly one song is selected
                if (selectedSongs.Count == 1)
                {
                    EditorComboBox.SelectedItem = string.IsNullOrEmpty(firstSong.AssignedEditor)
                        ? "None"
                        : firstSong.AssignedEditor;
                }
                else
                {
                    EditorComboBox.SelectedItem = null;
                }
            }
            else
            {
                SelectedGrade.Text = "";
                SelectedComment.Text = "";
                AssignButton.IsEnabled = true;
                EditorComboBox.IsEnabled = true;
                EditorComboBox.SelectedItem = "None";
            }
        }

        private void AssignSong_Click(object sender, RoutedEventArgs e)
        {
            var selectedSongs = SongsList.SelectedItems.Cast<Song>().ToList();
            if (!selectedSongs.Any()) return;

            var selectedEditor = EditorComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedEditor))
            {
                MessageBox.Show("Please select an editor from the dropdown.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var assignedTitles = new List<string>();
            var unassignedTitles = new List<string>();

            foreach (var song in selectedSongs)
            {
                // Skip reviewed songs
                if (!string.IsNullOrEmpty(song.Review)) continue;

                if (selectedEditor == "None")
                {
                    song.AssignedEditor = null;
                    unassignedTitles.Add(song.Title);
                }
                else
                {
                    song.AssignedEditor = selectedEditor;
                    assignedTitles.Add(song.Title);
                }
            }

            DataStore.SaveSongs();
            SongsList.Items.Refresh();

            var messages = new List<string>();
            if (assignedTitles.Any())
                messages.Add($"Assigned editor '{selectedEditor}' to:\n{string.Join("\n", assignedTitles)}");

            if (unassignedTitles.Any())
                messages.Add($"Unassigned editor from:\n{string.Join("\n", unassignedTitles)}");

            if (messages.Any())
                MessageBox.Show(string.Join("\n\n", messages),
                    "Assignment Update",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
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
