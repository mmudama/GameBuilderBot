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

            string response = "> " + GetResponseForEventRoll(definition, state, choice, 0);

            return response;
        }

        public string GetResponseForEventRoll(GameDefinition definition, GameState state, string choice, int depth)
        {
            depth++;

            if (depth > 20)
            {
                return "Too much nesting detected. Check your config file. Aborting!";
            }

            StringBuilder response = new StringBuilder();

            if (definition.ChoiceMap.ContainsKey(choice.ToLower()))
            {
                response.AppendLine((string.Format("**{0}**", choice)));
                Choice c = definition.ChoiceMap[choice.ToLower()];

                switch (c.Distribution)
                {
                    case "Weighted":
                        response.Append(GetResponseForWeightedChoice(definition, state, c, depth));
                        break;

                    case "All":
                        response.Append(GetResponseForDistributionAllChoice(definition, state, c, depth));
                        break;

                    default:
                        response.AppendLine(string.Format("\"{0}\" has an invalid Distribution", choice));
                        break;
                }
            }
            else
            {
                response.AppendLine(string.Format("unrecognized parameter **`{0}`**", choice));
            }

            if (response.Length == 0)
            {
                response.AppendLine("Something went wrong");
            }
            return response.ToString();
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

        // TODO make this a member of Choice
        private StringBuilder GetResponseForDistributionAllChoice(GameDefinition definition, GameState state, Choice c, int depth)
        {
            StringBuilder response = new StringBuilder();

            foreach (Outcome o in c.outcomeMap.Values)
            {
                if (o.ChildChoice == null)
                {
                    response.AppendLine("\t" + GetResponseForOutcome(state, o));
                }
                else
                {
                    response.Append(GetResponseForEventRoll(definition, state, o.ChildChoice.Name, depth));
                }
            }

            return response;
        }

        // TODO make this a member of outcome
        private string GetResponseForOutcome(GameState state, Outcome o)
        {
            string response;

            if (o.Rolls != null && o.Rolls.Length > 0)
            {
                var rolls = new List<int>();
                foreach (string expression in o.Rolls)
                {
                    rolls.Add(state.SetFieldValueByExpression(expression));
                }

                try
                {
                    response = string.Format(o.Text, rolls.Select(x => x.ToString()).ToArray());
                    response = string.Format("**{0}**", response);
                }
                catch (FormatException)
                {
                    response = "**!! Number of rolls specified does not match string format. Check your config file.!!**";
                }
            }
            else
            {
                response = o.Text;
            }

            return response;
        }

        // TODO make this a member of choice
        private StringBuilder GetResponseForWeightedChoice(GameDefinition definition, GameState state, Choice c, int depth)
        {
            StringBuilder response = new StringBuilder();

            int max = c.PossibleOutcomes.Length;
            int roll = DiceRollService.Roll(max) - 1;

            Outcome o = c.outcomeMap[c.PossibleOutcomes[roll]];

            string outcome = GetResponseForOutcome(state, o);

            if (max <= 1)
            {
                response.AppendLine(outcome);
            }
            else if (max == 2)
            {
                response.AppendLine(string.Format("Flipped a coin and got: **{0}**", outcome));
            }
            else
            {
                response.AppendLine(string.Format("[1d{0}: {1}] **{2}**", max, roll + 1, outcome));
            }

            if (o.ChildChoice != null)
            {
                response.Append(GetResponseForEventRoll(definition, state, o.ChildChoice.Name, depth));
            }

            return response;
        }

        public override string OneLinerHelp()
        {
            return "`!game <action>` will roll for action. Type `!game help` to see a complete list of the current available actions";
        }
    }
}
