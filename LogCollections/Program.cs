using System.Collections.Generic;
using System.IO;

namespace LogCollections
{
    internal class Program
    {
        /* TODO: Parse through the directory structure to get a list of all directories
         * which contain songs, and throw those into a list of Artist-Album-Song groupings.
         * Do the same with the porn as well. Possibly also what's on the terabyte.
         */
        const string Target = @"C:\Users\darth_000\SkyDrive\Documents";

        private static void Main()
        {

            var libraryList = ParseMusicCollection(@"C:\Users\darth_000\Music", @"F:\Music");
            DumpMusicCollection(libraryList);
            ParseAndDumpPorn(@"C:\Users\darth_000\Videos");
            //Console.ReadLine();

        }

        private static void ParseAndDumpPorn(string cUsersDarthVideos)
        {
            if (!Directory.Exists(cUsersDarthVideos)) return;
            var movies = Directory.GetFiles(cUsersDarthVideos, "*.*", SearchOption.AllDirectories);
            using (var pornLog = File.CreateText(Target + "/movies.txt"))
            {
                foreach (var movie in movies)
                {
                    pornLog.WriteLine(movie.Replace(cUsersDarthVideos+"\\", ""));
                }
            }
        }

        private static void DumpMusicCollection(Dictionary<string, Dictionary<string, HashSet<string>>> libraryList)
        {
            using (var musicLog = File.CreateText(Target + "/music.txt"))
            {
                foreach (var artist in libraryList)
                {
                    foreach (var album in artist.Value)
                    {
                        musicLog.WriteLine(artist.Key + " - " + album.Key);

                        //We probably have the full album.
                        if (album.Value.Count >= 4) continue;
                        foreach (var song in album.Value)
                        {
                            musicLog.WriteLine("\t" + song);

                        }
                    }
                    musicLog.WriteLine();

                }
            }
        }

        private static Dictionary<string, Dictionary<string, HashSet<string>>> ParseMusicCollection(params string[] directories)
        {

            var libraryList = new Dictionary<string, Dictionary<string, HashSet<string>>>();
            foreach (var libraryDir in directories)
            {
                if (!Directory.Exists(libraryDir)) continue;

                var songList = Directory.EnumerateFiles(libraryDir, "*.mp3", SearchOption.AllDirectories);
                foreach (var song in songList)
                {
                    var data = song.Replace(libraryDir, "").Split('\\');
                    var artistName = data[1];
                    var albumName = data[2];
                    var songName = data[3];
                    if (!libraryList.ContainsKey(artistName))
                        libraryList.Add(artistName, new Dictionary<string, HashSet<string>>());
                    if (!libraryList[artistName].ContainsKey(albumName))
                        libraryList[artistName].Add(albumName, new HashSet<string>());
                    if (libraryList[artistName][albumName].Contains(songName)) continue;
                    libraryList[artistName][albumName].Add(songName);
                }
            }
            return libraryList;
        }
    }
}
