using System;

namespace ConsoleApplication1
{
    public static class Program
    {
        public static void Main()
        {
            var main = new WoxSteam.Main();
            var query = new Wox.Plugin.Query();

            query.GetType().GetProperty("RawQuery")?.SetValue(query, "Arm", null);
            query.GetType().GetProperty("Search")?.SetValue(query, "Search", null);

            main.Init(null);
            main.Query(query);
            Console.WriteLine("Libraries: " + main.Steam.Libraries.Count);
            Console.WriteLine("Games: " + main.Steam.Games.Count);
            Console.WriteLine("Results: ");

            foreach (var game in main.Steam.Games)
            {
                Console.WriteLine(game.Name);
                Console.WriteLine(game.Icon);
            }
        }
    }
}