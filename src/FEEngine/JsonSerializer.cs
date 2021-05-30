using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FEEngine.Math;

namespace FEEngine
{
    /// <summary>
    /// A helper class for serializing and deserializing objects through <see cref="Newtonsoft.Json.JsonSerializer"/>
    /// </summary>
    public class JsonSerializer
    {
        private class Vec2Converter<T> : CustomCreationConverter<IVec2<T>> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            public override IVec2<T> Create(Type type)
            {
                return new GenericVec2<T>();
            }
        }
        public delegate void SerializationPlugin(Newtonsoft.Json.JsonSerializer serializer);
        /// <summary>
        /// Deserializes an object from the file at the specified path
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize as</typeparam>
        /// <param name="path">The path of the file to read</param>
        /// <param name="plugin">Runs after the see <see cref="Newtonsoft.Json.JsonSerializer"/> is created</param>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(string path, SerializationPlugin plugin = null)
        {
            TextReader textReader = new StreamReader(path);
            JsonReader reader = new JsonTextReader(textReader);
            Newtonsoft.Json.JsonSerializer serializer = new();
            plugin?.Invoke(serializer);
            // ill add more vec2 converters later
            serializer.Converters.Add(new Vec2Converter<int>());
            T value =  serializer.Deserialize<T>(reader);
            reader.Close();
            return value;
        }
        /// <summary>
        /// Serializes an object to the file at the specified path
        /// </summary>
        /// <param name="path">The path of the file to write</param>
        /// <param name="value">The object to serialize</param>
        /// <param name="plugin">Runs after the see <see cref="Newtonsoft.Json.JsonSerializer"/> is created</param>
        public static void Serialize(string path, object value, SerializationPlugin plugin = null)
        {
            TextWriter textWriter = new StreamWriter(path);
            JsonWriter writer = new JsonTextWriter(textWriter);
            Newtonsoft.Json.JsonSerializer serializer = new();
            plugin?.Invoke(serializer);
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, value);
            writer.Close();
        }
    }
}
