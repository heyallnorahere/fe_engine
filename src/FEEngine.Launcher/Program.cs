using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FEEngine.Launcher
{
    public static class Program
    {
        public class Options
        {
            [Option('g', "game", HelpText = "Loads a game immediately instead of in a menu")]
            public string? GameFileName { get; set; }
        }
        public static void Main(string[] args)
        {
            string? gameFileName = null;
            new Parser().ParseArguments<Options>(args).WithParsed(options =>
            {
                gameFileName = options.GameFileName;
            });
            var assemblies = new Dictionary<string, Assembly>();
            string directoryPath = Path.Join(Directory.GetCurrentDirectory(), "Games");
            string? autoloadKey = null;
            if (Directory.Exists(directoryPath))
            {
                foreach (string path in Directory.EnumerateFiles(directoryPath, "*.dll"))
                {
                    string filename = Path.GetFileName(path);
                    assemblies.Add(filename, Assembly.LoadFrom(path));
                }
            }
            if (gameFileName != null)
            {
                if (assemblies.ContainsKey(gameFileName))
                {
                    autoloadKey = gameFileName;
                }
            }
            var game = new Game("data/bindings.json");
            var frame = new GameFrame(assemblies, game, autoloadKey);
            game.Renderer.Root = frame;
            var player = new Player(game);
            game.Loop(player);
        }
    }
}
