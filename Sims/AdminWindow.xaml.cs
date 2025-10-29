using System.IO;
using System.Linq;
using System.Windows;

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

        private void AssignSong_Click(object sender, RoutedEventArgs e)
        {
            var selectedSong = SongsList.SelectedItem as Song;
            if (selectedSong == null)
            {
                MessageBox.Show("Please select a song first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedEditor = EditorComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedEditor))
            {
                MessageBox.Show("Please select an editor from the dropdown.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedSong.AssignedEditor = selectedEditor;

            DataStore.SaveSongs();

            MessageBox.Show($"Assigned '{selectedSong.Title}' to {selectedEditor}.",
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
