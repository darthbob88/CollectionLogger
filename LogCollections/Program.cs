using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogCollections
{
    internal class Program
    {
        static private string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        static private string[] TARGETS = { profile + @"\OneDrive\Documents",
            profile + @"\Dropbox\Public",
            profile + @"\Google Drive" };

        private static void Main()
        {
            ParseAndDumpMovies(@"/movies.xml", @"F:\Movies");
            ParseAndDumpComics(@"/comics.xml", @"F:\Comics");
            ParseAndDumpTV(@"/TV.xml", @"F:\TV Shows");
            ParseAndDumpPorn(@"/business_material.xml", profile + @"\Videos", @"F:\Videos");
            ParseAndDumpMusicCollection(@"/music.xml", profile + @"\Music", @"F:\Music");
            //Console.ReadLine();
        }
        private async static void ParseAndDumpMovies(string logFile, params string[] libraries)
        {
            if (!libraries.Where(Directory.Exists).Any())
                return;

            //Only one folder full of movies, after all.
            var movieList = PullCollection(libraries);

            using (var context = new mediaEntities())
            {
                var MovieTree = new XElement("MovieCollection");

                foreach (var movie in movieList)
                {
                    MovieTree.Add(new XElement("film", movie));
                    if (!context.movies.Any(film => film.movie_name == movie))
                        context.movies.Add(new movies { movie_name = movie });
                }

                await WriteFilesAsync(MovieTree, logFile);
                context.SaveChanges();
            }
        }

        private static async void ParseAndDumpComics(string logFile, params string[] libraries)
        {
            if (!libraries.Where(Directory.Exists).Any())
                return;

            var seriesList = PullCollection(libraries).GroupBy(item => item.Split('\\')[0]);

            using (var context = new mediaEntities())
            {
                var ComicTree = new XElement("ComicCollection");
                foreach (var series in seriesList)
                {
                    if (!context.comic_series.Any(item => item.series_name == series.Key))
                        context.comic_series.Add(new comic_series { series_name = series.Key });

                    var seriesNode = new XElement("series", new XAttribute("name", series.Key));
                    seriesNode.Add(series.Select(issue =>
                            new XElement("comic", Regex.Match(issue, "([^\\\\]+)$").Groups[0].Value)));

                    ComicTree.Add(seriesNode);
                }
                await WriteFilesAsync(ComicTree, logFile);
                context.SaveChanges();
            }
        }

        private async static void ParseAndDumpTV(string logFile, params string[] tvShows)
        {
            if (!tvShows.Where(Directory.Exists).Any())
                return;

            var seasons = PullCollection(tvShows).GroupBy(item => item.Split('\\')[0]);
            using (var context = new mediaEntities())
            {
                var TVTree = new XElement("TVCollection");
                foreach (var season in seasons)
                {
                    XElement seasonNode = new XElement("series", new XAttribute("name", season.Key));
                    var currentShow = context.tv_shows.Any(item => item.show_name == season.Key) ?
                        context.tv_shows.First(item => item.show_name == season.Key)
                        : context.tv_shows.Add(new tv_shows { show_name = season.Key });

                    foreach (var title in season.Select(episode => Regex.Match(episode, "([^\\\\]+)$").Groups[0].Value))
                    {
                        seasonNode.Add(new XElement("episode", title));
                        if (currentShow.tv_episodes.All(item => item.episode_name != title))
                            currentShow.tv_episodes.Add(new tv_episodes
                            {
                                episode_name = title,
                                tv_shows = currentShow
                            });
                    }
                    TVTree.Add(seasonNode);
                }
                await WriteFilesAsync(TVTree, logFile);
                context.SaveChanges();
            }
        }

        private static async void ParseAndDumpPorn(string logFile, params string[] pornStash)
        {
            if (!pornStash.Where(Directory.Exists).Any())
                return;

            var movies = PullCollection(pornStash);
            using (var context = new mediaEntities())
            {
                var MediaTree = new XElement("filecabinet");
                foreach (var videoName in movies)
                {
                    if (!context.porn.Any(item => item.video_name == videoName))
                        context.porn.Add(new porn { video_name = videoName });

                    //TODO Find some way of adding categories.
                    MediaTree.Add(new XElement("movie", videoName));
                }
                await WriteFilesAsync(MediaTree, logFile);
                context.SaveChanges();
            }
        }

        async private static void ParseAndDumpMusicCollection(string logFile, params string[] directories)
        {
            if (!directories.Where(Directory.Exists).Any())
                return;
            var musicList = PullCollection(directories).Where(item => item.EndsWith(".mp3")).Select(item => item.Split('\\')).GroupBy(item => item[0]);
            using (var context = new mediaEntities())
            {
                XElement MusicTree = new XElement("MusicCollection");
                foreach (var artist in musicList)
                {
                    var currentArtist = context.artist.Any(item => item.artist_name == artist.Key) ?
                        context.artist.First(item => item.artist_name == artist.Key) :
                            context.artist.Add(new artist { artist_name = artist.Key });

                    XElement artistNode = new XElement("artist", new XAttribute("name", artist.Key));

                    var albumList = artist.GroupBy(item => item[1]);
                    foreach (var album in albumList)
                    {
                        if (currentArtist.album.All(item => item.album_title != album.Key))
                            currentArtist.album.Add(new album { album_title = album.Key });

                        var albumNode = new XElement("album", new XAttribute("name", album.Key));
                        albumNode.Add(album.Select(song => new XElement("track", song[2])));
                        artistNode.Add(albumNode);
                    }
                    MusicTree.Add(artistNode);
                }
                context.SaveChanges();
                await WriteFilesAsync(MusicTree, logFile);
            }
        }

        private static IEnumerable<string> PullCollection(params string[] collections)
        {
            return collections.Where(Directory.Exists).Select(
                        directory =>
                            Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                                .Where(episode =>
                                        !(episode.EndsWith(".db") || episode.EndsWith(".txt") || episode.EndsWith(".srt") ||
                                          episode.EndsWith(".ini"))
                                          )
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
