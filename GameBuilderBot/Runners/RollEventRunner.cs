using System.Text;
using Discord.Commands;
using GameBuilderBot.Models;
using GameBuilderBot.Services;

namespace GameBuilderBot.Runners
{
    public class RollEventRunner : CommandRunner
    {
        protected string[] _variables;
        protected ExportService _exportService;

        public RollEventRunner(GameHandlingService gameHandler, ExportService exportService) : base(gameHandler)
        {
            _exportService = exportService;
        }

        public string RollEvent(string[] objects, SocketCommandContext discordContext)
        {
            if (objects.Length == 0) return OneLinerHelp();

            if (objects[0].ToLower().Equals("help")) return Help(discordContext);

            ulong channelId = discordContext.Channel.Id;
            string gameEventAsString = objects[0].ToLower();

            GameState state = _gameService.GetGameStateForActiveGame(channelId);
            GameDefinition definition = _gameService.GetGameDefinitionForChannelId(channelId);

            string response;

            if (definition.GameEventMap.ContainsKey(gameEventAsString))
            {
                GameEvent theEvent = definition.GameEventMap[gameEventAsString];

                response = $"Outcome of {gameEventAsString} event:\n{theEvent.GetResponseForEventRoll(definition, state, 0)}";
                _exportService.ExportGameState(state, discordContext);
            }
            else
            {
                response = Help(discordContext);
            }

            return response;
        }

        public string Help(SocketCommandContext discordContext)
        {
            StringBuilder sbResponse = new StringBuilder();
            GameDefinition definition = _gameService.GetGameDefinitionForChannelId(discordContext.Channel.Id);

            sbResponse.AppendLine("Available commands:");

            foreach (GameEvent c in definition.GameEventMap.Values)
            {
                if (c.IsPrimary)
                {
                    sbResponse.AppendFormat("`!game {0}`: {1}", c.Name, c.Description).AppendLine();
                }
            }
            return sbResponse.ToString();
        }

        public override string OneLinerHelp()
        {
            return "`!game <action>` will roll for action. Type `!game help` to see a complete list of the current available actions";
        }
    }
}
