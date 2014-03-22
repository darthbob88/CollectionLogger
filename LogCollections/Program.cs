using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LogCollections
{
    internal class Program
    {
        private const string TARGET = @"C:\Users\darth_000\SkyDrive\Documents";

        private static void Main()
        {

            ParseAndDumpMusicCollection(@"C:\Users\darth_000\Music", @"F:\Music");
            ParseAndDumpPorn(@"C:\Users\darth_000\Videos", @"F:\Videos");
            ParseAndDumpTV(@"F:\TV Shows");
            ParseAndDumpComics(@"/comics.xml", @"F:\Comics");
            //Console.ReadLine();

        }

        private static void ParseAndDumpComics(string logFile, params string[] libraries)
        {
            using (var comicsLog = File.CreateText(TARGET + logFile))
            {
                var seriesList = libraries.Where(Directory.Exists).
                    Select(library => Directory.GetFiles(library, "*.*", SearchOption.AllDirectories)
                        .Where(comic => !(comic.EndsWith(".db") || comic.EndsWith(".txt") || comic.EndsWith(".srt"))).Select(comic => comic.Replace(library + "\\", ""))
                        .GroupBy(comic => comic.Remove(Math.Max(0, comic.LastIndexOf("\\"))))).SelectMany(series => series);
                XElement ComicTree = new XElement("ComicCollection");
              foreach (var series in seriesList)
                    { XElement seriesNode = new XElement("series", new XAttribute("name", series.Key));
                    foreach (var comic in series)
                    {
                  

                        seriesNode.Add(new XElement("comic", comic.Substring(1 + Math.Max(0, comic.LastIndexOf("\\")))));
                    }
                    ComicTree.Add(seriesNode);
                }
                comicsLog.Write(ComicTree);
            }
        }

        private static void ParseAndDumpTV(params string[] tvShows)
        {
            using (var tvLog = File.CreateText(TARGET + "/TV.xml"))
            {
                var seasons = tvShows.Where(Directory.Exists)
                                     .Select(
                                         directory =>
                                         Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                                         .Where(episode => !(episode.EndsWith(".db") || episode.EndsWith(".txt") || episode.EndsWith(".srt")))
                                      .Select(episode => episode.Replace(directory + "\\", ""))
                                      .GroupBy(episode =>
                                                      episode.Remove(Math.Max(0, episode.LastIndexOf("\\")))
                                      ))
                                     .SelectMany(seasonList => seasonList);
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
                tvLog.Write(TVTree);
            }
        }

        private static void ParseAndDumpPorn(params string[] pornStash)
        {
            using (var pornLog = File.CreateText(TARGET + "/movies.txt"))
            {

                var movies = pornStash.Where(Directory.Exists)
                                         .Select(
                                             directory =>
                                             Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                                             .Where(episode => !(episode.EndsWith(".db") || episode.EndsWith(".txt")))
                                             .Select(episode => episode.Replace(directory + "\\", "")))
                                             .SelectMany(video => video);
                foreach (var movie in movies)
                {
                    pornLog.WriteLine(movie);
                }
            }
        }

        private static void ParseAndDumpMusicCollection(params string[] directories)
        {
            using (var musicLog = File.CreateText(TARGET + "/music.xml"))
            {
                XElement MusicTree = new XElement("MusicCollection");
                foreach (var album in
                    directories.Where(Directory.Exists)
                               .Select(
                                   libraryDir =>
                                   Directory.EnumerateFiles(libraryDir, "*.mp3", SearchOption.AllDirectories)
                                            .Select(song => song.Replace(libraryDir + "\\", ""))
                                      .GroupBy(song => song.Remove(Math.Max(0, song.LastIndexOf("\\")))
                                      ))
                               .SelectMany(albumList => albumList))
                {
                    XElement seasonNode = new XElement("album", new XAttribute("name", album.Key));
                    foreach (var song in album)
                    {
                        seasonNode.Add(new XElement("song", song.Substring(1 + Math.Max(0, song.LastIndexOf("\\")))));
                    }
                    MusicTree.Add(seasonNode);
                }
                musicLog.Write(MusicTree);
            }
        }
    }
}
