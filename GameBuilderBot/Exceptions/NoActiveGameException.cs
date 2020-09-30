using System;

namespace GameBuilderBot.Exceptions
{
    internal class NoActiveGameException : GameBuilderBotException
    {
        public NoActiveGameException() : base() { }

        public NoActiveGameException(string message) : base(message)
        {
            Console.WriteLine(message);
        }
    }
}
