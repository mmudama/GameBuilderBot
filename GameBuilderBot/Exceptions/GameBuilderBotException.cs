using System;

namespace GameBuilderBot.Exceptions
{
    public class GameBuilderBotException : Exception
    {
        public GameBuilderBotException(string message) : base(message) { }
    }
}
