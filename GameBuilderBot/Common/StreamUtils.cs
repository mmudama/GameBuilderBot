using System.IO;

namespace GameBuilderBot.Common
{
    public class StreamUtils
    {
        public static MemoryStream GetStreamFromString(string contents)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(contents);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
