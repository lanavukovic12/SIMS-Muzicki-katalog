using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MyFirstWpfApp
{
    public partial class UserWindow : Window
    {
        private string UserEmail;
        private ObservableCollection<Song> Playlist;

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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}