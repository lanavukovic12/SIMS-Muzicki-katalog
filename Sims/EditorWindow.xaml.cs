using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MyFirstWpfApp
{
    public partial class EditorWindow : Window
    {
        private string EditorEmail;
        private ObservableCollection<Song> AssignedSongsList;

        public EditorWindow(string email)
        {
            InitializeComponent();
            EditorEmail = email ?? throw new ArgumentNullException(nameof(email));
            LoadAssignedSongs();
        }

        private void LoadAssignedSongs()
        {
            // Load all songs assigned to this editor
            var songs = DataStore.Songs
                .Where(s => string.Equals(s.AssignedEditor, EditorEmail, StringComparison.OrdinalIgnoreCase))
                .ToList();

            AssignedSongsList = new ObservableCollection<Song>(songs);
            SongsList.ItemsSource = AssignedSongsList;

            // Minimal change: gray out reviewed songs
            SongsList.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in SongsList.Items)
                {
                    var lbi = SongsList.ItemContainerGenerator.ContainerFromItem(item) as System.Windows.Controls.ListBoxItem;
                    if (lbi != null && (item as Song)?.Grade != null)
                        lbi.Foreground = System.Windows.Media.Brushes.Gray;
                }
            });

            // Reset UI
            SelectedSongTitle.Text = "(none)";
            GradeBox.Text = "";
            ReviewBox.Text = "";
        }

        private void SongsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var song = SongsList.SelectedItem as Song;
            if (song != null)
            {
                SelectedSongTitle.Text = song.Title;
                GradeBox.Text = song.Grade?.ToString() ?? "";
                ReviewBox.Text = song.Review ?? "";

                // Disable Publish button if song already has a grade
                PublishReviewButton.IsEnabled = song.Grade == null;
            }
            else
            {
                SelectedSongTitle.Text = "(none)";
                GradeBox.Text = "";
                ReviewBox.Text = "";
                ReviewBox.Text = "";
                PublishReviewButton.IsEnabled = false;
            }
        }

        private void PublishReview_Click(object sender, RoutedEventArgs e)
        {
            var selectedSong = SongsList.SelectedItem as Song;
            if (selectedSong == null)
            {
                MessageBox.Show("Please select a song first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(GradeBox.Text))
            {
                MessageBox.Show("Please enter a grade.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(GradeBox.Text.Trim(), out int grade))
            {
                MessageBox.Show("Grade must be a number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (grade < 1 || grade > 5)
            {
                MessageBox.Show("Grade must be between 1 and 5.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // NEW: require comment as well
            if (string.IsNullOrWhiteSpace(ReviewBox.Text))
            {
                MessageBox.Show("Please enter a comment.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // update model
            selectedSong.Grade = grade;
            selectedSong.Review = ReviewBox.Text.Trim();

            DataStore.SaveSongs();

            AssignedSongsList.Remove(selectedSong);

            MessageBox.Show($"Review published for '{selectedSong.Title}'.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            SongsList.SelectedItem = null;
            SelectedSongTitle.Text = "(none)";
            GradeBox.Text = "";
            ReviewBox.Text = "";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            DataStore.SaveSongs();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
