using CommandLine;
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
            public string? GameFilename { get; set; }
        }
        public static void Main(string[] args)
        {
            string? gameFilename = null;
            new Parser().ParseArguments<Options>(args).WithParsed(options =>
            {
                gameFilename = options.GameFilename;
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
            if (gameFilename != null)
            {
                if (assemblies.ContainsKey(gameFilename))
                {
                    autoloadKey = gameFilename;
                }
            }
            var game = new Game("data/bindings.json");
            var player = new Player(game);
            var frame = new GameFrame(assemblies, game, player, autoloadKey);
            game.Renderer.Root = frame;
            game.Loop(player);
            frame.Loader?.Unload();
        }
    }
}
