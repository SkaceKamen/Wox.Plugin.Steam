using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WoxSteam
{
	/// <summary>
	/// Contains plugin options.
	/// </summary>
	public class Options
	{
		private readonly string filePath;

		[JsonProperty]
		public string SteamPath { get; set; }

		public Options(string path)
		{
			filePath = path;
		}

		/// <summary>
		/// Loads options saved to file if exists.
		/// </summary>
		public void Load()
		{
			// Skip loading if nothing was saved
			if (!File.Exists(filePath)) return;

			// Load stored options
			var loaded = JsonConvert.DeserializeObject<Options>(File.ReadAllText(filePath));

			// Save them here
			SteamPath = loaded.SteamPath;
		}

		/// <summary>
		/// Saves current values to file.
		/// </summary>
		public void Save()
		{
			File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
		}
	}
}
