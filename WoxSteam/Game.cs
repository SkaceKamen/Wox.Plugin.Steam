using System.IO;
using System.Net;
using NeXt.Vdf;
using WoxSteam.BinaryVdf;
using VdfInteger = NeXt.Vdf.VdfInteger;
using VdfString = NeXt.Vdf.VdfString;

namespace WoxSteam
{
	/// <summary>
	/// Represents single installed game and its details.
	/// </summary>
	public class Game
	{
		/// <summary>
		/// Contains informations stored in app manifest file.
		/// </summary>
		private readonly VdfTable manifest;
		/// <summary>
		/// Path where resources, like icons, should be stored when downloaded.
		/// </summary>
		private readonly string resourcesPath;

		/// <summary>
		/// Application id. Used to identify game on steam.
		/// </summary>
		public int Appid => ((VdfInteger) manifest.GetByName("appid")).Content;

		/// <summary>
		/// Game name.
		/// </summary>
		public string Name => ((VdfString) manifest.GetByName("name")).Content;

		/// <summary>
		/// Path to game icon. If the icon isn't already downloaded, it will be downloaded upon requesting this.
		/// </summary>
		public string Icon => LoadIcon();

		/// <summary>
		/// Game details loaded from appinfo.vdf
		/// </summary>
		public BinaryVdfItem Details { get; set; }

		public Game(string pathToManifest, string resourcesPath)
		{
			manifest = (VdfTable) VdfDeserializer.FromFile(pathToManifest).Deserialize();
			this.resourcesPath = resourcesPath;
		}

		/// <summary>
		/// Downloads game icon if present and not already downloaded.
		/// </summary>
		/// <returns>local path to cached game icon</returns>
		private string LoadIcon()
		{
			if (Details == null) return null;
            if (Details.GetString("clienticon") == null) return null;
			var icon = Details.GetString("clienticon") + ".ico";
			var cachePath = Path.Combine(resourcesPath, icon);

			if (File.Exists(cachePath)) return cachePath;

			using (var client = new WebClient())
			{
				client.DownloadFile(
					$"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/{Appid}/{icon}",
					cachePath
				);
			}

			return cachePath;
		}
	}
}
