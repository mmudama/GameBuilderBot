namespace GameBuilderBot.Common
{
    public class StringUtils
    {
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
