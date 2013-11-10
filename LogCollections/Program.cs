using System;
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

            ParseMusicCollection(@"C:\Users\darth_000\Music", @"F:\Music");

            Console.ReadLine();

        }

        private static void ParseMusicCollection(params string[] directories)
        {
            StreamWriter musicLog = File.CreateText(Target + "/music.txt");
            Dictionary<string, List<string>> libraryList = new Dictionary<string, List<string>>();
            foreach (string libraryDir in directories)
            {
                if (!Directory.Exists(libraryDir)) continue;

                var artistList = Directory.EnumerateDirectories(libraryDir);
                foreach (var artistDir in artistList)
                {
                    string artist = artistDir.Substring(artistDir.LastIndexOf('\\') + 1);
                    if (libraryList.ContainsKey(artist)) continue;

                    var albumList = Directory.EnumerateDirectories(artistDir);
                    foreach (var albumDir in albumList)
                    {
                        string[] songList = Directory.GetFiles(albumDir, "*.mp3");
                        if (songList.Length <= 1) continue;
                        string album = albumDir.Substring(albumDir.LastIndexOf('\\') + 1);
                        //Console.WriteLine(artist + " - " + album);
                        libraryList[artist].Add(album);
                    }

                }
            }

        }
    }
}
