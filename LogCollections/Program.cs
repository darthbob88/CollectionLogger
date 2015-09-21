using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogCollections
{
    /// <summary>
    /// Catalogs and dumps media collections to an SQLite database and XML files in three different cloud storage folders.
    /// Really wish I could do it in just one parameterized ParseAndOrganize method that works on movies/comics/etc.
    /// Everything's so similar in structure, but they're also too different in organization to have the same method handle both music and movies.
    /// TODO Add more media types and sources, possibly including ebooks and RSS feeds and such.
    /// TODO Add more storage locations or metadata.
    /// TODO Add better serialization as well. Will probably require adding more tables for metadata. 
    /// </summary>
    internal class Program
    {
        static private readonly string Profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        /// <summary>
        /// List of cloud storage folders to which our catalogs will be written. 
        /// TODO Move to external user-adjustable config, along with the library locations and logfiles.
        /// TODO Alternatively, find programmatic way to detect which cloud-storage folders are available.
        /// </summary>
        static private readonly string[] Targets = { Profile + @"\OneDrive\Documents",
            Profile + @"\Dropbox\Public",
            Profile + @"\Google Drive" };

        private static void Main()
        {
            ParseAndDumpMovies(@"/movies.xml", @"F:\Movies");
            ParseAndDumpComics(@"/comics.xml", @"F:\Comics");
            ParseAndDumpTV(@"/TV.xml", @"F:\TV Shows");
            ParseAndDumpPorn(@"/business_material.xml", Profile + @"\Videos", @"F:\Videos");
            ParseAndDumpMusicCollection(@"/music.xml", Profile + @"\Music", @"F:\Music");
            //Console.ReadLine();
        }
        /// <summary>
        /// Reads in a list of movie files for cataloging from <paramref name="libraries"/>.
        /// Writes list of files to XML file named <paramref name="logFile"/>, in directories specified in
        /// Targets field of class, and also writes to SQLite database for ease of use with other programs,
        /// including web frontend for this system.
        /// TODO Without excess cleverness, restructure as much of this as possible into a single function for all media sets.
        /// TODO Maybe an AddOrIgnore extension method for DbSet?
        /// </summary>
        /// <param name="logFile">File name to which a list of movies will be written for backup purposes.</param>
        /// <param name="libraries">Directory/ies containing movie collection.</param>
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
        /// <summary>
        /// Reads in a list of comics for cataloging from <paramref name="libraries"/>.
        /// Writes list of files to XML file named <paramref name="logFile"/>, in directories specified in
        /// Targets field of class, and also writes to SQLite database for ease of use with other programs,
        /// including web frontend for this system.
        /// TODO Without excess cleverness, restructure as much of this as possible into a single function for all media sets.
        /// TODO Maybe an AddOrIgnore extension method for DbSet?
        /// TODO Add table for individual issues.
        /// </summary>
        /// <param name="logFile">File name to which a list of comics will be written for backup purposes.</param>
        /// <param name="libraries">Directory/ies containing media collection.</param>
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
        /// <summary>
        /// Reads in a list of TV episodes for cataloging from <paramref name="libraries"/>.
        /// Writes list of files to XML file named <paramref name="logFile"/>, in directories specified in
        /// Targets field of class, and also writes to SQLite database for ease of use with other programs,
        /// including web frontend for this system.
        /// TODO Without excess cleverness, restructure as much of this as possible into a single function for all media sets.
        /// TODO Maybe an AddOrIgnore extension method for DbSet?
        /// TODO Find some way to parse out the episode number for ID purposes.
        /// </summary>
        /// <param name="logFile">File name to which a list of TV episodes will be written for backup purposes.</param>
        /// <param name="libraries">Directory/ies containing TV shows.</param>
        private async static void ParseAndDumpTV(string logFile, params string[] libraries)
        {
            if (!libraries.Where(Directory.Exists).Any())
                return;

            var seasons = PullCollection(libraries).GroupBy(item => item.Split('\\')[0]);
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
        /// <summary>
        /// Reads in a list of video files for cataloging from <paramref name="libraries"/>.
        /// Writes list of files to XML file named <paramref name="logFile"/>, in directories specified in
        /// Targets field of class, and also writes to SQLite database for ease of use with other programs,
        /// including web frontend for this system.
        /// TODO Without excess cleverness, restructure as much of this as possible into a single function for all media sets.
        /// TODO Maybe an AddOrIgnore extension method for DbSet?
        /// </summary>
        /// <param name="logFile">File name to which a list of movies will be written for backup purposes.</param>
        /// <param name="libraries">Directory/ies containing movie collection.</param>
        private static async void ParseAndDumpPorn(string logFile, params string[] libraries)
        {
            if (!libraries.Where(Directory.Exists).Any())
                return;

            var movies = PullCollection(libraries);
            using (var context = new mediaEntities())
            {
                var MediaTree = new XElement("filecabinet");
                foreach (var videoName in movies)
                {
                    if (!context.porn.Any(item => item.video_name == videoName))
                        context.porn.Add(new porn { video_name = videoName });

                    //TODO Find some way of re-adding categories.
                    MediaTree.Add(new XElement("movie", videoName));
                }
                await WriteFilesAsync(MediaTree, logFile);
                context.SaveChanges();
            }
        }
        /// <summary>
        /// Reads in a list of music files for cataloging from <paramref name="libraries"/>.
        /// Writes list of files to XML file named <paramref name="logFile"/>, in directories specified in
        /// Targets field of class, and also writes to SQLite database for ease of use with other programs,
        /// including web frontend for this system.
        /// TODO Without excess cleverness, restructure as much of this as possible into a single function for all media sets.
        /// TODO Maybe an AddOrIgnore extension method for DbSet?
        /// TODO Add song table to database.
        /// </summary>
        /// <param name="logFile">File name to which a list of music tracks will be written for backup purposes.</param>
        /// <param name="libraries">Directory/ies containing music collection.</param>
        async private static void ParseAndDumpMusicCollection(string logFile, params string[] libraries)
        {
            if (!libraries.Where(Directory.Exists).Any())
                return;
            var musicList = PullCollection(libraries).Where(item => item.EndsWith(".mp3")).Select(item => item.Split('\\')).GroupBy(item => item[0]);
            using (var context = new mediaEntities())
            {
                XElement MusicTree = new XElement("MusicCollection");
                foreach (var artist in musicList)
                {
                    var currentArtist = context.artist.Any(item => item.artist_name == artist.Key) ?
                        context.artist.First(item => item.artist_name == artist.Key) :
                            context.artist.Add(new artist { artist_name = artist.Key });

                    XElement artistNode = new XElement("artist", new XAttribute("name", artist.Key));

                    var albumList = artist.GroupBy(song => song[1]);
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
        /// <summary>
        /// Pulls a list of filenames for media objects in the given collection(s).
        /// Includes filters to exclude unnecessary metadata files, and trims filenames to
        /// just the media file and hierarchy within the library. artist/album/MP3,
        /// without C:long/path/to/media/library/
        /// TODO Idunno. Find better way of organizing the filters and selects?
        /// </summary>
        /// <param name="collections">A list of directories that contain parts of a media collection.</param>
        /// <returns>An IEnumerable of filenames, ready to be organized into a better hierarchy by the calling method.</returns>
        private static IEnumerable<string> PullCollection(params string[] collections)
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
        /// <summary>
        /// Utility methods for asynchronously dumping our catalogs to XML files. 
        /// TODO Generalize to accept other objects.
        /// </summary>
        /// <param name="data">Data to write. </param>
        /// <param name="file">File name to create.</param>
        /// <returns></returns>
        public async static Task WriteFilesAsync(XElement data, string file)
        {
            var tasks = Targets.Where(Directory.Exists).Select(f => WriteAsync(data, f + file));
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
