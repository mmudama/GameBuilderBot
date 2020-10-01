namespace GameBuilderBot.Common
{
    /// <summary>
    /// Convenience functionality related to strings
    /// </summary>
    public class StringUtils
    {
        /// <summary>
        /// Creates a string that is suitable for use within a filename.
        /// There is no way to reverse the process and retrieve the original string.
        /// </summary>
        /// <param name="term">The source string</param>
        /// <returns>A new string with problematic characters removed</returns>
        public static string SanitizeForFileName(string term)
        {
            string sanitizedGameName = term;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                sanitizedGameName = sanitizedGameName.Replace(c, '_');
            }

            sanitizedGameName = sanitizedGameName.Replace(' ', '_');

            return sanitizedGameName;
        }
    }
}
