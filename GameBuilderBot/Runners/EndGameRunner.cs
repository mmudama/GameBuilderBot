using System;
using System.Collections.Generic;
using GameBuilderBot.Models;
using GameBuilderBot.Services;

namespace GameBuilderBot.Runners
{
    public class EndGameRunner : CommandRunner
    {
        protected string[] _variables;
        GameHandlingService _gameHandler;

        public EndGameRunner(GameHandlingService gameHandler) : base(gameHandler)
        {
            _gameHandler = gameHandler;
        }

        public string EndGame(ulong channelId)
        {
            return _gameHandler.RemoveGameDefinitionForChannelId(channelId);
        }

        public override string OneLinerHelp()
        {
            return "`!end` will end the current game. You can start again with !start";
        }
    }
}
