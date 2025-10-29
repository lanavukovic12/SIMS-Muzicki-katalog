using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MyFirstWpfApp
{
    public static class DataStore
    {
        private static readonly string SongsFilePath = "songs.csv";
        private static readonly string PlaylistFilePath = "user_playlist.csv";

        public static ObservableCollection<Song> Songs { get; set; } = new ObservableCollection<Song>();

        static DataStore()
        {
            LoadSongs();
        }

        // ---------------- SONGS CSV ----------------
        public static void SaveSongs()
        {
            try
            {
                using (var writer = new StreamWriter(SongsFilePath))
                {
                    // header
                    writer.WriteLine("Title,AssignedEditor,Message,Review,Grade");
                    foreach (var s in Songs)
                    {
                        string title = EscapeCsv(s.Title);
                        string assignedEditor = EscapeCsv(s.AssignedEditor);
                        string message = EscapeCsv(s.Message);
                        string review = EscapeCsv(s.Review);
                        string grade = s.Grade?.ToString() ?? "";

                        writer.WriteLine($"{title},{assignedEditor},{message},{review},{grade}");
                    }
                }
            }
            catch { }
        }

        public static void LoadSongs()
        {
            Songs.Clear();

            if (!File.Exists(SongsFilePath))
            {
                // default songs
                Songs.Add(new Song { Title = "Song A" });
                Songs.Add(new Song { Title = "Song B" });
                Songs.Add(new Song { Title = "Song C" });
                return;
            }

            var lines = File.ReadAllLines(SongsFilePath).Skip(1);
            foreach (var line in lines)
            {
                var parts = ParseCsvLine(line);
                if (parts.Length >= 5)
                {
                    int? grade = null;
                    if (int.TryParse(parts[4], out int g))
                        grade = g;

                    Songs.Add(new Song
                    {
                        Title = parts[0],
                        AssignedEditor = parts[1],
                        Message = parts[2],
                        Review = parts[3],
                        Grade = grade
                    });
                }
            }
        }

        // ---------------- USER PLAYLIST CSV ----------------
        public static void SaveUserPlaylist(string userEmail, IEnumerable<Song> playlist)
        {
            var existingLines = new List<string>();
            if (File.Exists(PlaylistFilePath))
                existingLines = File.ReadAllLines(PlaylistFilePath).Where(l => !l.StartsWith(userEmail + ",")).ToList();

            using (var writer = new StreamWriter(PlaylistFilePath))
            {
                // write other users' data
                foreach (var line in existingLines)
                    writer.WriteLine(line);

                // write current user's playlist
                foreach (var song in playlist)
                    writer.WriteLine($"{userEmail},{EscapeCsv(song.Title)}");
            }
        }

        public static List<Song> LoadUserPlaylist(string userEmail)
        {
            var result = new List<Song>();
            if (!File.Exists(PlaylistFilePath))
                return result;

            var lines = File.ReadAllLines(PlaylistFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length >= 2 && parts[0] == userEmail)
                {
                    var song = Songs.FirstOrDefault(s => s.Title == parts[1]);
                    if (song != null)
                        result.Add(song);
                }
            }
            return result;
        }

        // ---------------- CSV Helpers ----------------
        private static string EscapeCsv(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Contains(",") || text.Contains("\"") || text.Contains("\n"))
            {
                text = text.Replace("\"", "\"\"");
                return $"\"{text}\"";
            }
            return text;
        }

        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var current = new System.Text.StringBuilder();

            foreach (char c in line)
            {
                if (c == '"' && !inQuotes)
                {
                    inQuotes = true;
                }
                else if (c == '"' && inQuotes)
                {
                    inQuotes = false;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            result.Add(current.ToString());
            return result.ToArray();
        }
    }
}
