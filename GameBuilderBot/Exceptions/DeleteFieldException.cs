namespace GameBuilderBot.Exceptions
{
    /// <summary>
    /// To signal that something went wrong attempting to delete
    /// a field from the current game state
    /// </summary>
    public class DeleteFieldException : GameBuilderBotException
    {
        /// <summary>
        /// To signal that something went wrong attempting to delete
        /// a field from the current game state
        /// </summary>
        /// <param name="message">Informative message</param>
        public DeleteFieldException(string message) : base(message)
        {
        }
    }
}
