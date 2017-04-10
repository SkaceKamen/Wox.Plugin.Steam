using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin;
using WoxSteam;

namespace ConsoleApplication1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var main = new WoxSteam.Main();
			var query = new Query();

			query.GetType().GetProperty("RawQuery").SetValue(query, "Arm", null);
			query.GetType().GetProperty("Search").SetValue(query, "Search", null);

			main.Init(null);
			var results = main.Query(query);

			Console.WriteLine("Libraries: " + main.Steam.Libraries.Count);
			Console.WriteLine("Games: " + main.Steam.Games.Count);
			Console.WriteLine("Results: ");
			foreach (var result in results)
			{
				Console.WriteLine(result.Title);
			}

			Console.ReadKey();
		}
	}
}
