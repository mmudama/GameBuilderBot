using GameBuilderBot.Services;
using System;

namespace GameBuilderBot.Runners
{
    /// <summary>
    /// Inherit from this class to implement the business logic that backs bot commands.
    /// TODO actually should this just be an interface to require OneLinerHelp to be implemented? 
    /// </summary>
    public abstract class CommandRunner
    {
        protected GameHandlingService _gameService;

        public CommandRunner(GameHandlingService gameHandlingService)
        {
            _gameService = gameHandlingService;
        }

        /// <summary>
        /// TODO it turns out that testing argument length is a lot more nuanced than this.
        /// Either make it more sophisticated or rip it out.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="minLength"></param>
        public void TestArgLength(string[] args, int minLength)
        {
            if (args.Length < minLength)
            {
                throw new ArgumentException(OneLinerHelp());
            }
        }

        /// <summary>
        /// TODO maybe pass the bot command (e.g. "start", "set", etc to the constructor
        /// to standardize this behavior a little more ... hm.
        /// </summary>
        /// <returns>A one line help message suitable for printing to the user</returns>
        abstract public string OneLinerHelp();
    }
}
