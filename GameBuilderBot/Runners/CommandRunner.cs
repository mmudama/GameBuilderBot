using GameBuilderBot.Services;
using System;

namespace GameBuilderBot.Runners
{
    public abstract class CommandRunner
    {
        protected GameHandlingService _gameService;

        public CommandRunner(GameHandlingService gameHandlingService)
        {
            _gameService = gameHandlingService;
        }

        public void TestArgLength(string[] args, int minLength)
        {
            if (args.Length < minLength)
            {
                throw new ArgumentException(OneLinerHelp());
            }
        }

        abstract public string OneLinerHelp();
    }
}
