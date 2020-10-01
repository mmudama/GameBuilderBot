using System;

namespace GameBuilderBot.Exceptions
{
    /// <summary>
    /// Used to signify an exception originating from within the GameBuilderBot application
    /// </summary>
    public class GameBuilderBotException : Exception
    {
        /// <summary>
        /// Used to signify an exception originating from within the GameBuilderBot application
        /// </summary>
        public GameBuilderBotException() : base() { }

        /// <summary>
        /// Used to signify an exception originating from within the GameBuilderBot application
        /// </summary>
        /// <param name="message">A helpful message</param>
        public GameBuilderBotException(string message) : base(message) { }
    }
}
