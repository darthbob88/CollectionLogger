using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogCollections
{
    internal class Program
    {
        static private readonly string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        static private readonly string[] TARGETS = { profile + @"\OneDrive\Documents",
            profile + @"\Dropbox\Public",
            profile + @"\Google Drive" };

        private static void Main()
        {
            ParseAndDumpMovies(@"/movies.xml", @"F:\Movies");
            ParseAndDumpMusicCollection(@"/music.xml", profile + @"\Music", @"F:\Music");
            ParseAndDumpPorn(@"/business_material.xml", profile + @"\Videos", @"F:\Videos");
            ParseAndDumpTV(@"/TV.xml", @"F:\TV Shows");
            ParseAndDumpComics(@"/comics.xml", @"F:\Comics");
            //Console.ReadLine();
        }
        private async static void ParseAndDumpMovies(string logFile, params string[] libraries)
        {
            if (!libraries.Where(Directory.Exists).Any())
                return;

            //Only one folder full of movies, after all.
            var movieList = PullCollection(libraries).First();
            XElement MovieTree = new XElement("MovieCollection");
            foreach (var movie in movieList)
            {
                MovieTree.Add(new XElement("film", movie));
            }
            await WriteFilesAsync(MovieTree, logFile);
        }
        private async static void ParseAndDumpComics(string logFile, params string[] libraries)
        {
            if (!libraries.Where(Directory.Exists).Any())
                return;

            var seriesList = PullCollection(libraries);
            XElement ComicTree = new XElement("ComicCollection");
            foreach (var series in seriesList)
            {
                XElement seriesNode = new XElement("series", new XAttribute("name", series.Key));
                foreach (var comic in series)
                {
                    seriesNode.Add(new XElement("comic", comic.Substring(1 + Math.Max(0, comic.LastIndexOf("\\")))));
                }
                ComicTree.Add(seriesNode);
            }
            await WriteFilesAsync(ComicTree, logFile);
        }

        private async static void ParseAndDumpTV(string logFile, params string[] tvShows)
        {
            if (!tvShows.Where(Directory.Exists).Any())
                return;

            var seasons = PullCollection(tvShows);
            XElement TVTree = new XElement("TVCollection");
            foreach (var season in seasons)
            {
                XElement seasonNode = new XElement("season", new XAttribute("name", season.Key));
                foreach (var episode in season)
                {
                    seasonNode.Add(new XElement("episode", episode.Substring(1 + Math.Max(0, episode.LastIndexOf("\\")))));
                }
                TVTree.Add(seasonNode);
            }
            await WriteFilesAsync(TVTree, logFile);
        }

        async private static void ParseAndDumpPorn(string logFile, params string[] pornStash)
        {
            if (!pornStash.Where(Directory.Exists).Any())
                return;

            var movies = PullCollection(pornStash);
            XElement MediaTree = new XElement("filecabinet");
            foreach (var category in movies)
            {
                XElement fileNode = new XElement("category", new XAttribute("name", category.Key));
                foreach (var episode in category)
                {
                    fileNode.Add(new XElement("movie", episode.Substring(Math.Max(0, 1 + episode.LastIndexOf("\\")))));
                }
                MediaTree.Add(fileNode);
            }
            await WriteFilesAsync(MediaTree, logFile);
        }

        async private static void ParseAndDumpMusicCollection(string logFile, params string[] directories)
        {
            if (!directories.Where(Directory.Exists).Any())
                return;
            XElement MusicTree = new XElement("MusicCollection");
            foreach (var album in PullCollection(directories))
            {
                XElement seasonNode = new XElement("album", new XAttribute("name", album.Key));
                foreach (var song in album)
                {
                    if (song.EndsWith(".mp3"))
                    {
                        seasonNode.Add(new XElement("song", song.Substring(1 + Math.Max(0, song.LastIndexOf("\\")))));
                    }
                }
                if (!seasonNode.IsEmpty)
                {
                    MusicTree.Add(seasonNode);
                }
            }
            await WriteFilesAsync(MusicTree, logFile);
        }

        private static IEnumerable<IGrouping<string, string>> PullCollection(params string[] collections)
        {
            return collections.Where(Directory.Exists).Select(
                        directory =>
                            Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                                .Where(episode =>
                                        !(episode.EndsWith(".db") || episode.EndsWith(".txt") || episode.EndsWith(".srt") || episode.EndsWith(".ini"))
                                        || episode.EndsWith(".nfo") || episode.EndsWith(".tbn") || episode.EndsWith(".jpg"))
                                .Select(episode => episode.Replace(directory + "\\", "")))
                                .SelectMany(seasonList => seasonList)
                                .Distinct();

        }
        public async static Task WriteFilesAsync(XElement data, string file)
        {
            var tasks = TARGETS.Where(Directory.Exists).Select(f => WriteAsync(data, f + file));
            await Task.WhenAll(tasks);
        }

        private static async Task WriteAsync(XElement data, string path)
        {
            using (var fs = new StreamWriter(path))
            {
                await fs.WriteAsync(data.ToString());
            }
        }
    }
}
