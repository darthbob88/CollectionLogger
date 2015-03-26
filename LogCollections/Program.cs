﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogCollections
{
    internal class Program
    {
        static private string[] TARGETS = new String[] { @"C:\Users\Raymond\OneDrive\Documents",
            @"C:\Users\Raymond\Dropbox\Public",
            @"C:\Users\Raymond\Google Drive" };

        private static void Main()
        {
            ParseAndDumpMusicCollection(@"/music.xml", @"C:\Users\darth_000\Music", @"F:\Music");
            ParseAndDumpPorn(@"/business_material.xml", @"C:\Users\darth_000\Videos", @"F:\Videos");
            ParseAndDumpTV(@"/TV.xml", @"F:\TV Shows");
            ParseAndDumpComics(@"/comics.xml", @"F:\Comics");
            //Console.ReadLine();
        }

        private async static void ParseAndDumpComics(string logFile, params string[] libraries)
        {
            if (0 > libraries.Where(Directory.Exists).Count())
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
            if (0 > tvShows.Where(Directory.Exists).Count())
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
            if (0 > pornStash.Where(Directory.Exists).Count())
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
            if (0 > directories.Where(Directory.Exists).Count())
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
                                       .Where(episode => !(episode.EndsWith(".db") || episode.EndsWith(".txt") || episode.EndsWith(".srt") || episode.EndsWith(".ini")))
                                    .Select(episode => episode.Replace(directory + "\\", ""))).SelectMany(seasonList => seasonList)
                                    .Distinct().GroupBy(episode => episode.Remove(Math.Max(0, episode.LastIndexOf("\\"))));
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
