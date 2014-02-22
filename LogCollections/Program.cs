using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LogCollections
{
    internal class Program
    {
        private const string Target = @"C:\Users\darth_000\SkyDrive\Documents";

        private static void Main()
        {

            ParseAndDumpMusicCollection(@"C:\Users\darth_000\Music", @"F:\Music");
            ParseAndDumpPorn(@"C:\Users\darth_000\Videos", @"F:\Videos");
            ParseAndDumpTV(@"F:\TV Shows");
            //Console.ReadLine();

        }

        private static void ParseAndDumpTV(params string[] tvShows)
        {
            using (var tvLog = File.CreateText(Target + "/TV.xml"))
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
            using (var pornLog = File.CreateText(Target + "/movies.txt"))
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
            using (var musicLog = File.CreateText(Target + "/music.xml"))
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
