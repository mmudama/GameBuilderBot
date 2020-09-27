using System.IO;

namespace GameBuilderBot.Common
{
    public class StreamUtils
    {
        public static MemoryStream GetStreamFromString(string response)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(response);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
