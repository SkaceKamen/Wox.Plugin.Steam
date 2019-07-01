using System.IO;
using System.Net;
using NeXt.Vdf;

namespace WoxSteam
{
    /// <summary>
    /// Represents single installed game and its details.
    /// </summary>
    public class Game
    {
        private const string IconUrl = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/{0}/{1}";

        /// <summary>
        /// Contains informations stored in app manifest file.
        /// </summary>
        private readonly VdfTable _manifest;

        /// <summary>
        /// Path where resources, like icons, should be stored when downloaded.
        /// </summary>
        private readonly string _resourcesPath;

        private static readonly WebClient WebClient = new WebClient();

        /// <summary>
        /// Application id. Used to identify game on steam.
        /// </summary>
        public int Appid => ((VdfInteger) _manifest.GetByName("appid")).Content;

        /// <summary>
        /// Game name.
        /// </summary>
        public string Name => ((VdfString) _manifest.GetByName("name")).Content;

        /// <summary>
        /// Path to game icon. If the icon isn't already downloaded, it will be downloaded upon requesting this.
        /// </summary>
        public string Icon => LoadIcon();

        /// <summary>
        /// Game details loaded from appinfo.vdf
        /// </summary>
        public AppInfo Details { get; set; }

        public Game(string pathToManifest, string resourcesPath)
        {
            _manifest = (VdfTable) VdfDeserializer.FromFile(pathToManifest).Deserialize();
            _resourcesPath = resourcesPath;
        }

        /// <summary>
        /// Downloads game icon if present and not already downloaded.
        /// </summary>
        /// <returns>local path to cached game icon</returns>
        private string LoadIcon()
        {
            var clientIcon = Details?.ClientIcon;
            if (clientIcon == null)
            {
                return null;
            }

            var icon = $"{clientIcon}.ico";
            var cachePath = Path.Combine(_resourcesPath, icon);

            if (File.Exists(cachePath))
            {
                return cachePath;
            }

            WebClient.DownloadFile(string.Format(IconUrl, Appid, icon), cachePath);
            return cachePath;
        }
    }
}