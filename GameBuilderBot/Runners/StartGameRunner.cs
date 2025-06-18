using System;
using System.Collections.Generic;
using GameBuilderBot.Models;
using GameBuilderBot.Services;

namespace GameBuilderBot.Runners
{
    public class StartGameRunner : CommandRunner
    {
        protected string[] _variables;

        public StartGameRunner(GameHandlingService gameHandler) : base(gameHandler)
        {
        }

        public bool DisplayAllGames(string[] inputs, out List<GameDefinition> allGames)
        {
            if (inputs.Length == 0)
            {
                allGames = _gameService.GetAllGameDefinitions();
                return true;
            }

            allGames = null;
            return false;
        }

        public string StartGame(string[] inputs, ulong channelId)
        {
            if (inputs.Length > 0 && int.TryParse(inputs[0], out int selection) && selection > 0)
            {
                _gameService.SetGameDefinitionForChannelId(channelId, selection - 1);
                return _gameService.GetGameDefinitionForChannelId(channelId).Name;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Unrecognized game");
            }
        }

        public bool RestoreGame(ulong channelId)
        {
            return _gameService.LoadGameState(channelId);
        }

        public override string OneLinerHelp()
        {
            return "`!start` will let you choose a game to play";
        }
    }
}
