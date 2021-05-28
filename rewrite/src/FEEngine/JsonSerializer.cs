using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FEEngine.Math;

namespace FEEngine
{
    public class JsonSerializer
    {
        private class Vec2Converter<T> : CustomCreationConverter<IVec2<T>> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            public override IVec2<T> Create(Type type)
            {
                return new GenericVec2<T>();
            }
        }
        public delegate void SerializationPlugin(ref Newtonsoft.Json.JsonSerializer serializer);
        public static T Deserialize<T>(string path, SerializationPlugin plugin = null)
        {
            TextReader textReader = new StreamReader(path);
            JsonReader reader = new JsonTextReader(textReader);
            Newtonsoft.Json.JsonSerializer serializer = new();
            plugin?.Invoke(ref serializer);
            // ill add more vec2 converters later
            serializer.Converters.Add(new Vec2Converter<int>());
            T value =  serializer.Deserialize<T>(reader);
            reader.Close();
            return value;
        }
        public static void Serialize(string path, object value)
        {
            TextWriter textWriter = new StreamWriter(path);
            JsonWriter writer = new JsonTextWriter(textWriter);
            Newtonsoft.Json.JsonSerializer serializer = new();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, value);
            writer.Close();
        }
    }
}
