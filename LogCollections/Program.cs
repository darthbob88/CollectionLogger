using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogCollections
{
    internal class Program
    {
        private const string Target = @"C:\Users\darth_000\SkyDrive\Documents";

        private static void Main()
        {

            ParseAndDumpMusicCollection(@"C:\Users\darth_000\Music", @"F:\Music");
            ParseAndDumpPorn(@"C:\Users\darth_000\Videos", @"F:\Videos");
            ParseAndDumpTV(@"F:\TV Shows", @"C:\Users\Public\Videos");
            //Console.ReadLine();

        }

        private static void ParseAndDumpTV(params string[] tvShows)
        {
            using (var tvLog = File.CreateText(Target + "/TV.txt"))
            {
                var seasons = tvShows.Where(Directory.Exists)
                                     .Select(
                                         directory =>
                                         Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                                         .Where(episode => !(episode.EndsWith(".db") || episode.EndsWith(".txt")))
                                      .Select(episode => episode.Replace(directory + "\\", ""))
                                      .GroupBy(episode =>
                                                      episode.Remove(Math.Max(0, episode.LastIndexOf("\\")))
                                      ))
                                     .SelectMany(seasonList => seasonList);
                foreach (var season in seasons)
                {
                    tvLog.WriteLine(season.Key);
                    if (season.Count() < 4)
                    {
                        foreach (var episode in season)
                        {
                            tvLog.WriteLine("\t" + episode);
                        }
                    }
                }
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
            using (var musicLog = File.CreateText(Target + "/music.txt"))
            {
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
                    musicLog.WriteLine(album.Key);
                    if (album.Count() < 4)
                    {
                        foreach (var song in album)
                        {
                            musicLog.WriteLine("\t" + song);
                        }
                    }
                }
            }
        }
    }
}
