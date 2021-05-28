using System.IO;
using Newtonsoft.Json;

namespace FEEngine
{
    public class JsonSerializer
    {
        public delegate void SerializationPlugin(ref Newtonsoft.Json.JsonSerializer serializer);
        public static T Deserialize<T>(string path, SerializationPlugin plugin = null)
        {
            TextReader textReader = new StreamReader(path);
            JsonReader reader = new JsonTextReader(textReader);
            Newtonsoft.Json.JsonSerializer serializer = new();
            plugin?.Invoke(ref serializer);
            T value =  serializer.Deserialize<T>(reader);
            reader.Close();
            return value;
        }
        public static void Serialize(string path, object value)
        {
            TextWriter textWriter = new StreamWriter(path);
            if (textWriter == null)
            {
                throw new System.Exception();
            }
            JsonWriter writer = new JsonTextWriter(textWriter);
            Newtonsoft.Json.JsonSerializer serializer = new();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, value);
            writer.Close();
        }
    }
}
