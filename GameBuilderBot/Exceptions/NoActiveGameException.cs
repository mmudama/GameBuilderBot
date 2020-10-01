using System;

namespace GameBuilderBot.Exceptions
{
    /// <summary>
    /// Used to signal that there is no game definition or state currently loaded.
    /// Most bot behaviors require that a game is currently in progress (loaded).
    /// </summary>
    public class NoActiveGameException : GameBuilderBotException
    {
        /// <summary>
        /// Used to signal that there is no game definition or state currently loaded.
        /// Most bot behaviors require that a game is currently in progress (loaded).
        /// </summary>
        public NoActiveGameException() : base() { }

        /// <summary>
        /// Used to signal that there is no game definition or state currently loaded.
        /// Most bot behaviors require that a game is currently in progress (loaded).
        /// </summary>
        /// <param name="message">An informative message</param>
        public NoActiveGameException(string message) : base(message)
        {
            Console.WriteLine(message);
        }
    }
}
