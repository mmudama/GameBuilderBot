using Discord.Commands;
using GameBuilderBot.Models;
using GameBuilderBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBuilderBot.Runners
{
    public class RollEventRunner : CommandRunner
    {
        protected string[] _variables;

        public RollEventRunner(GameHandlingService gameHandler) : base(gameHandler)
        {
        }

        public string RollEvent(string[] objects, SocketCommandContext discordContext)
        {
            if (objects.Length == 0) return OneLinerHelp();

            if (objects[0].ToLower().Equals("help")) return Help(discordContext);

            ulong channelId = discordContext.Channel.Id;
            string choice = objects[0];

            GameState state = _gameService.GetGameStateForActiveGame(channelId);
            GameDefinition definition = _gameService.GetGameDefinitionForChannelId(channelId);

            string response;

            if (definition.ChoiceMap.ContainsKey(choice.ToLower()))
            {
                Choice theChoice = definition.ChoiceMap[choice];

                response = "> " + theChoice.GetResponseForEventRoll(definition, state, 0);
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

            foreach (Choice c in definition.ChoiceMap.Values)
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
