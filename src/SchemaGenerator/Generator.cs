using System.IO;
using FEEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace SchemaGenerator
{
    public class Generator
    {
        private struct Settings
        {
            public string SchemaDirectory { get; set; }
            public DirectoryInfo SchemaDirectoryInfo { get; set; }
        }
        private static void WriteSchema<T>(string name = null)
        {
            string path = settings.SchemaDirectoryInfo.FullName + "/" + (name ?? typeof(T).Name) + ".json";
            JSchema schema = generator.Generate(typeof(T));
            TextWriter textWriter = new StreamWriter(path);
            JsonWriter jsonWriter = new JsonTextWriter(textWriter);
            schema.WriteTo(jsonWriter);
            jsonWriter.Close();
        }
        public static void Main(string[] args)
        {
            settings = new Settings();
            if (args.Length >= 1)
            {
                settings.SchemaDirectory = args[0];
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
            WriteSchema<Register<Map>>("Maps");
            WriteSchema<Register<Unit>>("Units");
            WriteSchema<Register<Item>>("Items");
            WriteSchema<Register<Tile>>("Tiles");
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
