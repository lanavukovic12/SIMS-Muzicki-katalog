using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MyFirstWpfApp
{
    public static class DataStore
    {
        private static readonly string SongsFilePath = "songs.csv";
        private static readonly string UserPlaylistsFilePath = "user_playlists.csv";

        public static ObservableCollection<Song> Songs { get; set; } = new ObservableCollection<Song>();
        public static List<string> Editors = new List<string>
        {
            "editor@gmail.com",
            "editor2@gmail.com",
            "music.editor@gmail.com"
        };

        static DataStore()
        {
            LoadSongs();
        }

        // ---------------- SONGS ----------------
        public static void SaveSongs()
        {
            try
            {
                using (var writer = new StreamWriter(SongsFilePath))
                {
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
                    if (int.TryParse(parts[4], out int g)) grade = g;

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

        // ---------------- USER PLAYLISTS ----------------
        // Dictionary: userEmail -> (playlistName -> list of songs)
        public static void SaveUserPlaylists(string userEmail, Dictionary<string, ObservableCollection<Song>> playlists)
        {
            var existingLines = new List<string>();
            if (File.Exists(UserPlaylistsFilePath))
                existingLines = File.ReadAllLines(UserPlaylistsFilePath)
                                    .Where(l => !l.StartsWith(userEmail + ",")).ToList();

            using (var writer = new StreamWriter(UserPlaylistsFilePath))
            {
                // write other users
                foreach (var line in existingLines)
                    writer.WriteLine(line);

                // write current user's playlists
                foreach (var kvp in playlists)
                {
                    string playlistName = EscapeCsv(kvp.Key);
                    foreach (var song in kvp.Value)
                        writer.WriteLine($"{userEmail},{playlistName},{EscapeCsv(song.Title)}");
                }
            }
        }

        public static Dictionary<string, ObservableCollection<Song>> LoadUserPlaylists(string userEmail)
        {
            var result = new Dictionary<string, ObservableCollection<Song>>();
            if (!File.Exists(UserPlaylistsFilePath)) return result;

            var lines = File.ReadAllLines(UserPlaylistsFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length >= 3 && parts[0] == userEmail)
                {
                    string playlistName = parts[1];
                    string songTitle = parts[2];

                    if (!result.ContainsKey(playlistName))
                        result[playlistName] = new ObservableCollection<Song>();

                    var song = Songs.FirstOrDefault(s => s.Title == songTitle);
                    if (song != null)
                        result[playlistName].Add(song);
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
                if (c == '"' && !inQuotes) inQuotes = true;
                else if (c == '"' && inQuotes) inQuotes = false;
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else current.Append(c);
            }

            result.Add(current.ToString());
            return result.ToArray();
        }
    }
}
