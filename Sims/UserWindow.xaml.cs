using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MyFirstWpfApp
{
    public partial class UserWindow : Window
    {
        private string UserEmail;
        private ObservableCollection<Song> Playlist;
        private string PlaylistName = "My Playlist";

        public UserWindow(string email)
        {
            InitializeComponent();
            UserEmail = email;

            // učitavanje svih pesama
            AllSongsList.ItemsSource = DataStore.Songs;
            // učitavanje plejlisti korisnika
            var loadedSongs = DataStore.LoadUserPlaylist(UserEmail);
            Playlist = new ObservableCollection<Song>(loadedSongs);
            PlaylistList.ItemsSource = Playlist;
            PlaylistName = DataStore.LoadPlaylistName();
            PlaylistNameBox.Text = PlaylistName;
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var selectedSongs = AllSongsList.SelectedItems.Cast<Song>().ToList();

            foreach (var song in selectedSongs)
            {
                if (!Playlist.Contains(song))
                    Playlist.Add(song);
            }

            DataStore.SaveUserPlaylist(UserEmail, Playlist);
        }

        private void RemoveFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var selectedSongs = PlaylistList.SelectedItems.Cast<Song>().ToList();

            foreach (var song in selectedSongs)
            {
                Playlist.Remove(song);
            }

            DataStore.SaveUserPlaylist(UserEmail, Playlist);
        }
        private void RenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            string newName = PlaylistNameBox.Text.Trim();

            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Playlist name cannot be empty.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PlaylistName = newName;
            MessageBox.Show($"Playlist renamed to \"{PlaylistName}\"!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Optional: save name to a file so it stays after restart
            DataStore.SavePlaylistName(PlaylistName);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}