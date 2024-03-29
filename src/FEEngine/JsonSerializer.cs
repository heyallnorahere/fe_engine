﻿using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    /// <summary>
    /// A helper class for serializing and deserializing objects through <see cref="Newtonsoft.Json.JsonSerializer"/>
    /// </summary>
    public class JsonSerializer
    {
        private class Vec2Converter : CustomCreationConverter<Vector2>
        {
            public override Vector2 Create(Type type)
            {
                return new Vector2();
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
        public static T? DeserializeFile<T>(string path, SerializationPlugin? plugin = null)
        {
            TextReader textReader = new StreamReader(path);
            return DeserializeImpl<T>(textReader, plugin);
        }
        public static T? DeserializeString<T>(string data, SerializationPlugin? plugin = null)
        {
            TextReader textReader = new StringReader(data);
            return DeserializeImpl<T>(textReader, plugin);
        }
        private static T? DeserializeImpl<T>(TextReader textReader, SerializationPlugin? plugin)
        {
            JsonReader reader = new JsonTextReader(textReader);
            Newtonsoft.Json.JsonSerializer serializer = new();
            plugin?.Invoke(serializer);
            serializer.Converters.Add(new Vec2Converter());
            T? value = serializer.Deserialize<T>(reader);
            reader.Close();
            return value;
        }
        /// <summary>
        /// Serializes an object to the file at the specified path
        /// </summary>
        /// <param name="path">The path of the file to write</param>
        /// <param name="value">The object to serialize</param>
        /// <param name="plugin">Runs after the see <see cref="Newtonsoft.Json.JsonSerializer"/> is created</param>
        public static void Serialize(string path, object value, SerializationPlugin? plugin = null)
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
