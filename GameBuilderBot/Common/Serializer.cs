using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace GameBuilderBot.Common
{
    /// <summary>
    /// The file format to which (from which) the call will serialize (deserialize) the object
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// https://www.json.org/example.html
        /// </summary>
        JSON,

        /// <summary>
        /// https://yaml.org/start.html
        /// </summary>
        YAML
    }

    /// <summary>
    /// Convenience class to centralize common (de)serialization functionality
    /// Supports JSON and YAML
    /// </summary>
    public class Serializer
    {
        private readonly YamlDotNet.Serialization.Serializer YamlSerializer;
        private readonly YamlDotNet.Serialization.Deserializer YamlDeserializer;

        /// <summary>
        /// Convenience class to centralize common (de)serialization functionality
        /// Supports JSON and YAML
        /// </summary>
        public Serializer()
        {
            YamlSerializer = new YamlDotNet.Serialization.Serializer();
            YamlDeserializer = new YamlDotNet.Serialization.Deserializer();
        }

        /// <summary>
        /// Takes an object of type T and serializes it to a file
        /// </summary>
        /// <typeparam name="T">Object type to be serialized</typeparam>
        /// <param name="o">The object</param>
        /// <param name="type">The file format to use</param>
        /// <param name="fileName">The fully qualified destination path</param>
        public void SerializeToFile<T>(T o, FileType type, string fileName)
        {
            string contents = SerializeToString(o, type);
            var streamWriter = new StreamWriter(fileName);
            streamWriter.Write(contents);
            streamWriter.Close();
        }

        /// <summary>
        /// Takes an object of type T and serializes it to a string
        /// </summary>
        /// <typeparam name="T">Object type to be serialized</typeparam>
        /// <param name="o">The Object</param>
        /// <param name="type">The format to use</param>
        /// <returns>a string containing the serialized version of the object</returns>
        public string SerializeToString<T>(T o, FileType type)
        {
            if (type == FileType.YAML)
            {
                return YamlSerializer.Serialize(o);
            }
            else if (type == FileType.JSON)
            {
                return JsonConvert.SerializeObject(o);
            }
            else throw new InvalidEnumArgumentException(string.Format("Unsupported FileType {0}", type));
        }

        /// <summary>
        /// Read a file and returns an object constructed from the file
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="fileName">The fully qualified file path</param>
        /// <param name="type">The format type</param>
        /// <returns>An object of type T</returns>
        public T DeserializeFromFile<T>(string fileName, FileType type)
        {
            string fileContents = File.ReadAllText(fileName);

            if (type == FileType.YAML)
            {
                return YamlDeserializer.Deserialize<T>(fileContents);
            }
            else if (type == FileType.JSON)
            {
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            else throw new InvalidEnumArgumentException(string.Format("Unsupported FileType {0}", type));
        }
    }
}
