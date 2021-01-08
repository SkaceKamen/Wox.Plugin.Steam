using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WoxSteam
{
	/// <summary>
	/// Represents one steam library path.
	/// </summary>
	public class Library
	{
		/// <summary>
		/// Regex used to match manifest files.
		/// </summary>
		private static readonly Regex AppMetaRegex = new Regex("appmanifest_[^\\.]*\\.acf", RegexOptions.IgnoreCase);

		/// <summary>
		/// Path to this library.
		/// </summary>
		private readonly string path;
		/// <summary>
		/// Reference to parent steam helper.
		/// </summary>
		private readonly Steam steam;

		/// <param name="parent">Builder of this library</param>
		/// <param name="path">Path to library</param>
		public Library(Steam parent, string path)
		{
			this.path = path;
			steam = parent;
		}

		public override string ToString()
		{
			return "Library(" + path + ")";
		}

		/// <summary>
		/// Iterates installed games.
		/// </summary>
		/// <returns>games installed in this library</returns>
		public IEnumerable<Game> GetGames()
		{
			var dir = new DirectoryInfo(Path.Combine(path, "steamapps"));

			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				if (AppMetaRegex.IsMatch(fileInfo.Name))
				{
					Game game = null;
					try
					{
						game = new Game(fileInfo.FullName, steam.CachePath);
					}
					catch (Exception) { } // There can be empty/corrupt library files. Ignore these.

					if (game != null)
						yield return game;
				}
			}
		}
	}
}
