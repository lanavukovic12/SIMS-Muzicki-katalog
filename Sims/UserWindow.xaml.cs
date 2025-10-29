using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MyFirstWpfApp
{
    public partial class UserWindow : Window
    {
        private string UserEmail;

        // Store all playlists for this user
        private Dictionary<string, ObservableCollection<Song>> UserPlaylists;

        // Current selected playlist
        private string CurrentPlaylistName;

        public UserWindow(string email)
        {
            InitializeComponent();
            UserEmail = email;

            AllSongsList.ItemsSource = DataStore.Songs;

            // Load all playlists for user
            UserPlaylists = DataStore.LoadUserPlaylists(UserEmail);

            // If no playlist exists, create default
            if (!UserPlaylists.Any())
                UserPlaylists["My Playlist"] = new ObservableCollection<Song>();

            // Populate playlist selector
            PlaylistSelector.ItemsSource = UserPlaylists.Keys;

            // Select first playlist by default
            CurrentPlaylistName = UserPlaylists.Keys.First();
            PlaylistSelector.SelectedItem = CurrentPlaylistName;
            PlaylistList.ItemsSource = UserPlaylists[CurrentPlaylistName];
            PlaylistNameBox.Text = CurrentPlaylistName;
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var selectedSongs = AllSongsList.SelectedItems.Cast<Song>().ToList();
            var currentPlaylist = UserPlaylists[CurrentPlaylistName];

            int addedCount = 0;

            foreach (var song in selectedSongs)
            {
                if (!currentPlaylist.Contains(song))
                {
                    currentPlaylist.Add(song);
                    addedCount++;
                }
            }

            DataStore.SaveUserPlaylists(UserEmail, UserPlaylists);

            if (addedCount > 0)
                MessageBox.Show($"{addedCount} song(s) added to playlist \"{CurrentPlaylistName}\".",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("No songs were added (they may already be in the playlist).",
                    "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RemoveFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var selectedSongs = PlaylistList.SelectedItems.Cast<Song>().ToList();
            var currentPlaylist = UserPlaylists[CurrentPlaylistName];

            int removedCount = 0;

            foreach (var song in selectedSongs)
            {
                if (currentPlaylist.Remove(song))
                    removedCount++;
            }

            DataStore.SaveUserPlaylists(UserEmail, UserPlaylists);

            if (removedCount > 0)
                MessageBox.Show($"{removedCount} song(s) removed from playlist \"{CurrentPlaylistName}\".",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("No songs were removed.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void RenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            string newName = PlaylistNameBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Playlist name cannot be empty.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentPlaylistName == null || newName == CurrentPlaylistName)
                return;

            if (UserPlaylists.ContainsKey(newName))
            {
                MessageBox.Show("A playlist with this name already exists.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var songs = UserPlaylists[CurrentPlaylistName];
            UserPlaylists.Remove(CurrentPlaylistName);
            UserPlaylists[newName] = songs;

            CurrentPlaylistName = newName;
            PlaylistSelector.ItemsSource = null;
            PlaylistSelector.ItemsSource = UserPlaylists.Keys;
            PlaylistSelector.SelectedItem = newName;

            DataStore.SaveUserPlaylists(UserEmail, UserPlaylists);

            MessageBox.Show($"Playlist renamed to \"{newName}\"!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PlaylistSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (PlaylistSelector.SelectedItem == null) return;

            CurrentPlaylistName = PlaylistSelector.SelectedItem.ToString();
            PlaylistList.ItemsSource = UserPlaylists[CurrentPlaylistName];
            PlaylistNameBox.Text = CurrentPlaylistName;
        }

        private void NewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string newPlaylistName = "New Playlist";
            int counter = 1;

            // Ensure unique name
            while (UserPlaylists.ContainsKey(newPlaylistName))
            {
                counter++;
                newPlaylistName = $"New Playlist {counter}";
            }

            // Create empty playlist
            UserPlaylists[newPlaylistName] = new ObservableCollection<Song>();

            // Update selector and select new playlist
            PlaylistSelector.ItemsSource = null;
            PlaylistSelector.ItemsSource = UserPlaylists.Keys;
            PlaylistSelector.SelectedItem = newPlaylistName;

            DataStore.SaveUserPlaylists(UserEmail, UserPlaylists);
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPlaylistName == null) return;

            // Prevent deleting the last playlist
            if (UserPlaylists.Count <= 1)
            {
                MessageBox.Show("You must have at least one playlist.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete the playlist \"{CurrentPlaylistName}\"?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            // Remove playlist
            UserPlaylists.Remove(CurrentPlaylistName);

            // Select another playlist (first one)
            CurrentPlaylistName = UserPlaylists.Keys.First();
            PlaylistSelector.ItemsSource = null;
            PlaylistSelector.ItemsSource = UserPlaylists.Keys;
            PlaylistSelector.SelectedItem = CurrentPlaylistName;
            PlaylistList.ItemsSource = UserPlaylists[CurrentPlaylistName];
            PlaylistNameBox.Text = CurrentPlaylistName;

            DataStore.SaveUserPlaylists(UserEmail, UserPlaylists);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
