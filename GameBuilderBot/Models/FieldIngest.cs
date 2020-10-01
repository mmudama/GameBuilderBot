namespace GameBuilderBot.Models
{
    /// <summary>
    /// FieldIngest objects are created when deserializing files into GameFile objects.
    /// They represent variables immediately available to the user by default as part of a game
    /// </summary>
    /// <seealso cref="GameFile"/>
    /// <seealso cref="Field"/>
    public class FieldIngest
    {
        /// <summary>
        /// <seealso cref="Field"/>
        /// </summary>
        public string Expression;

        /// <summary>
        /// <seealso cref="Field"/>
        /// </summary>
        public object Value;

        /// <summary>
        /// <seealso cref="Field"/>
        /// </summary>
        public string Type;

        /// <summary>
        /// Default constructor required for deserialization
        /// </summary>
        public FieldIngest() { }
    }
}
