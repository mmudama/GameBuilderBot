using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace GameBuilderBot.Common
{
    public enum FileType
    {
        JSON,
        YAML
    }

    public class Serializer
    {
        protected YamlDotNet.Serialization.Serializer YamlSerializer;
        protected YamlDotNet.Serialization.Deserializer YamlDeserializer;

        public Serializer()
        {
            YamlSerializer = new YamlDotNet.Serialization.Serializer();
            YamlDeserializer = new YamlDotNet.Serialization.Deserializer();
        }

        public void SerializeToFile<T>(T o, FileType type, string fileName)
        {
            string contents = SerializeToString(o, type);
            var streamWriter = new StreamWriter(fileName);
            streamWriter.Write(contents);
            streamWriter.Close();
        }

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
