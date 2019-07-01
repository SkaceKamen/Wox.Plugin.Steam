using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Win32;
using Newtonsoft.Json;
using NeXt.Vdf;
using WoxSteam.BinaryVdf;

namespace WoxSteam
{
    /// <summary>
    /// Steam helper class.
    /// </summary>
    public class Steam
    {
        /// <summary>
        /// Path used to cache icons and stuff.
        /// </summary>
        public string CachePath { get; set; } = "./Cache/";

        /// <summary>
        /// Path to steam installation root.
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// List of all current steam libraries.
        /// </summary>
        public List<Library> Libraries { get; } = new List<Library>();

        /// <summary>
        /// List of all currently installed games.
        /// </summary>
        public List<Game> Games { get; } = new List<Game>();

        /// <summary>
        /// Loads list of installed games.
        /// </summary>
        public void Load()
        {
            // Create cache if needed
            if (!File.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }

            // Load basic list of games
            if (RootPath == null)
            {
                LoadPath();
            }

            LoadLibraries();
            LoadGames();
            LoadGamesDetails();
        }

        /// <summary>
        /// Tries to assign game details from appinfo to individual games.
        /// </summary>
        private void LoadGamesDetails()
        {
            var appinfo = LoadAppinfo();
            foreach (var game in Games)
            {
                var id = (uint) game.Appid;
                if (appinfo.TryGetValue(id, out var item))
                {
                    game.Details = item;
                }
            }
        }

        /// <summary>
        /// Tries to locate steam installation path.
        /// </summary>
        private void LoadPath()
        {
            var regPath = "SOFTWARE\\Valve\\Steam";
            RootPath = "C:\\Program Files\\Steam";

            if (Environment.Is64BitOperatingSystem)
            {
                regPath = "SOFTWARE\\Wow6432Node\\Valve\\Steam";
                RootPath = "C:\\Program Files (x86)\\Steam";
            }

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(regPath))
                {
                    var o = key?.GetValue("InstallPath");
                    if (o != null)
                    {
                        RootPath = (string) o;
                    }
                }
            }
            finally
            {
                if (!Directory.Exists(RootPath))
                {
                    throw new Exception("Failed to locate steam installation.");
                }
            }
        }

        /// <summary>
        /// Tries to games details from steam appinfo.vdf file.
        /// </summary>
        private Dictionary<uint, AppInfo> LoadAppinfo()
        {
            // Basic appinfo file informations
            var appinfoPath = Path.Combine(RootPath, "appcache", "appinfo.vdf");
            var currentHash = Md5(appinfoPath);

            // Appinfo is cached to speedup loading
            var cache = Path.Combine(CachePath, "appinfo.json");
            var cacheHashFile = Path.Combine(CachePath, "appinfo.vdf.md5");
            string cacheHash = null;

            // Hash is used to determine if appinfo recently changed
            if (File.Exists(cacheHashFile))
            {
                cacheHash = File.ReadAllText(cacheHashFile);
            }

            if (!File.Exists(cache) || cacheHash != null && cacheHash != currentHash)
            {
                var apps = Reader.Read(appinfoPath).ToDictionary(pair => pair.Key, pair => AppInfo.From(pair.Value));
                var text = JsonConvert.SerializeObject(apps);
                File.WriteAllText(cache, text);
                File.WriteAllText(cacheHashFile, currentHash);
                return apps;
            }
            else
            {
                var text = File.ReadAllText(cache);
                return JsonConvert.DeserializeObject<Dictionary<uint, AppInfo>>(text);
            }
        }

        /// <summary>
        /// Loads paths to all user steam libraries.
        /// </summary>
        private void LoadLibraries()
        {
            Libraries.Clear();
            Libraries.Add(new Library(this, RootPath));

            var librariesPath = Path.Combine(RootPath, "steamapps", "libraryfolders.vdf");

            if (!File.Exists(librariesPath))
            {
                return;
            }

            var folders = (VdfTable) VdfDeserializer.FromFile(librariesPath).Deserialize();
            var index = 1;
            while (folders.TryGetValue(index.ToString(), out var value))
            {
                Libraries.Add(new Library(this, ((VdfString) value).Content));
                index++;
            }
        }

        /// <summary>
        /// Loads list of installed games.
        /// </summary>
        private void LoadGames()
        {
            Games.Clear();

            foreach (var lib in Libraries)
            {
                Games.AddRange(lib.GetGames().ToList());
            }
        }

        /// <summary>
        /// Helper function generating MD5 hash of file.
        /// </summary>
        /// <param name="file">path to file</param>
        /// <returns>md5 hash of file</returns>
        private static string Md5(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                return BitConverter.ToString(new MD5Cng().ComputeHash(stream)).Replace("-", string.Empty);
            }
        }
    }
}