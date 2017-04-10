using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Wox.Plugin;

namespace WoxSteam
{
    public class Main : IPlugin, ISettingProvider
    {
		/// <summary>
		/// Steam helper library.
		/// </summary>
		public Steam Steam { get; private set; }

		/// <summary>
		/// Contains plugin options.
		/// </summary>
		public Options Options { get; set; }

		/// <summary>
		/// Called when users enters some input
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
	    public List<Result> Query(Query query)
	    {
		    return Steam.Games
			    .Where(game => game.Name.ToLower().Contains(query.RawQuery.ToLower()))
				.Select(game => new Result()
			    {
				    Title = game.Name,
					SubTitle = "Steam game",
					IcoPath = game.Icon,
					Score = 100,
					Action = context =>
					{
						System.Diagnostics.Process.Start(Path.Combine(Steam.RootPath, "steam.exe"), "-applaunch " + game.Appid);
						return true;
					}
			    })
				.ToList();
	    }

		/// <summary>
		/// Called to initialize the plugin.
		/// </summary>
		/// <param name="context"></param>
	    public void Init(PluginInitContext context)
	    {
			// Create steam library helper
			// Use plugin directory if possible
		    Steam = new Steam
		    {
			    CachePath = context != null ? Path.Combine(context.CurrentPluginMetadata.PluginDirectory, "Cache") : "Cache"
		    };

			// Load options
		    Options = new Options(context != null ? Path.Combine(context.CurrentPluginMetadata.PluginDirectory, "options.json") : "options.json");
			Options.Load();

			// Apply steam path option if somewhat correct
		    if (Options.SteamPath != null && Directory.Exists(Options.SteamPath))
		    {
			    Steam.RootPath = Options.SteamPath;
		    }

		    // Loads game list
		    Steam.Load();

			// Update options value, no matter the source
		    Options.SteamPath = Steam.RootPath;
			Options.Save();
	    }

	    public Control CreateSettingPanel()
	    {
		    return new SettingsControl(Options);
	    }
    }
}
