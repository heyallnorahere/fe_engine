using System;
using System.IO;
using FEEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using CommandLine;

namespace SchemaGenerator
{
    public static class Generator
    {
        public class Options
        {
            [Option('d', "directory", HelpText = "The directory to write schemas to")]
            public string? SchemaDirectory { get; set; }
        }
        private struct Settings
        {
            public string SchemaDirectory { get; set; }
            public DirectoryInfo SchemaDirectoryInfo { get; set; }
        }
        private static void WriteSchema<T>(string? name = null)
        {
            string title = name ?? typeof(T).Name;
            string path = Path.Join(settings.SchemaDirectoryInfo.FullName, title + ".json");
            Console.WriteLine("Writing schema: {0}", path);
            JSchema schema = generator.Generate(typeof(T));
            TextWriter textWriter = new StreamWriter(path);
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            schema.Title = title;
            schema.WriteTo(jsonWriter);
            jsonWriter.Close();
            Console.WriteLine("Wrote schema: {0}", path);
        }
        private static void WriteRegisterSchema<T>(string? name = null) where T : class, IRegisteredObject<T>
        {
            string typeName = typeof(T).Name;
            WriteSchema<Register<T>>(name ?? typeName + "s");
        }
        public static void Main(string[] args)
        {
            settings = new Settings();
            Options? options = null;
            new Parser().ParseArguments<Options>(args).WithParsed(opt => options = opt);
            if (options?.SchemaDirectory != null)
            {
                settings.SchemaDirectory = options.SchemaDirectory;
            }
            else
            {
                settings.SchemaDirectory = "schemas";
            }
            if (!Directory.Exists(settings.SchemaDirectory))
            {
                settings.SchemaDirectoryInfo = Directory.CreateDirectory(settings.SchemaDirectory);
            }
            else
            {
                settings.SchemaDirectoryInfo = new(settings.SchemaDirectory);
            }
            WriteRegisterSchema<Map>();
            WriteRegisterSchema<Unit>();
            WriteRegisterSchema<Item>();
            WriteRegisterSchema<Tile>();
            WriteRegisterSchema<Battalion>();
            WriteSchema<InputManager.KeyBindings>();
        }
        private static JSchemaGenerator CreateGenerator()
        {
            JSchemaGenerator jSchemaGenerator = new();
            jSchemaGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());
            return jSchemaGenerator;
        }
        private static Settings settings;
        private static readonly JSchemaGenerator generator = CreateGenerator();
    }
}
