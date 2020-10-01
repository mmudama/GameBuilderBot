using System.IO;

namespace GameBuilderBot.Common
{
    /// <summary>
    /// Convenience class for functionality related to streaming
    /// </summary>
    public class StreamUtils
    {
        /// <summary>
        /// Creates a MemoryStream that contains the specified contents
        /// </summary>
        /// <param name="contents">string to be streamed</param>
        /// <returns>A stream to read from</returns>
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
