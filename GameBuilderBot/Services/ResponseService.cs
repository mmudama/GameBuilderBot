using Antlr4.Runtime.Tree;
using Discord.Commands;
using GameBuilderBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBuilderBot.Services
{
    public class ResponseService
    {
        private readonly Config _config;

        public ResponseService(Config config)
        {
            _config = config;
        }

        public string Help()
        {
            var sb = new StringBuilder()
                .AppendLine("> **Help:**");

            foreach (string k in _config.ChoiceMap.Keys)
            {
                Choice c = _config.ChoiceMap[k];
                if (c.IsPrimary)
                {
                    sb.AppendLine(String.Format("`!game {0}`: {1}", k, c.Description));
                }
            }

            return sb.ToString();
        }


        public Task Summarize(SocketCommandContext context)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Choice c in _config.ChoiceMap.Values)
            {
                sb.AppendLine(c.GetSummary());
            }

            string response = sb.ToString();

            if (response.Length < 2000)
            {
                return context.Channel.SendMessageAsync(response);
            }
            else
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(response);
                writer.Flush();
                stream.Position = 0;

                string fileName = string.Format("{0}_summary.txt", context.Channel.Name);
                return context.Channel.SendFileAsync(stream, fileName, "The summary is too long to send as a DM; " +
                    "sending it to you as a file instead.");

            }
        }

        internal string Evaluate(string expression)
        {
            expression = _config.Interpret(expression);

            var response = new StringBuilder("> Evaluate:").AppendLine();

            int value = -1;

            try
            {
                value = DiceRollService.Roll(expression);
            }
            catch (Exception)
            {
                response.AppendFormat("Failure attempting to evaluate `{0}`", expression).AppendLine();
            }

            response.AppendFormat("`{0} = {1}`", expression, value).AppendLine();
            return response.ToString();
        }

        internal string Get(string[] objects)
        {
            StringBuilder sb = new StringBuilder();

            // TODO consolidate "all" vs identified output logic

            if (objects.Length == 0)
            {
                sb.AppendLine("> **All Fields**:");

                foreach (string key in _config.Fields.Keys)
                {

                    sb.Append("`")
                        .Append(key)
                        .Append(": ");

                    if (_config.Fields[key].Value != null)
                    {
                        sb.AppendFormat("{0}", _config.Fields[key].Value);
                    }
                    else
                    {
                        sb.Append("**Undefined**");
                    }

                    if (_config.Fields[key].Expression != null)
                    {
                        sb.AppendFormat(" ({0})", _config.Fields[key].Expression);
                    }

                    sb.AppendLine("`");
                }
            }
            else
            {
                sb.AppendLine("> **Requested Fields**:");
                foreach (string s in objects)
                {
                    string value;
                    if (_config.Fields.ContainsKey(s) && _config.Fields[s].Value != null)
                    {
                        value = _config.Fields[s].Value.ToString();
                    }
                    else
                    {
                        value = "**Undefined**";
                    }

                    sb.Append("`")
                        .Append(s)
                        .Append(": ")
                        .AppendFormat("{0}", value);

                    if (_config.Fields.ContainsKey(s) && _config.Fields[s].Expression != null)
                    {
                        sb.AppendFormat(" ({0})", _config.Fields[s].Expression);
                    }

                    sb.AppendLine("`");
                }
            }

            return sb.ToString();
        }

        internal string Set(string[] objects)
        {
            var response = new StringBuilder("> set").AppendLine();

            var errorResponse = "> set syntax: `!set <name> <integer value>`" +
                "\n OR `!set <name> <expression>` (like 1d4 or 1+5)";


            int value = -1;

            if (objects.Length != 2) return errorResponse;

            string expression = objects[1];


            if (!int.TryParse(expression, out value))
            {
                try
                {
                    expression = _config.Interpret(expression);
                    value = DiceRollService.Roll(expression);
                }
                catch (Exception)
                {
                    return errorResponse;
                }
            }

            string name = objects[0].ToLower();

            if (_config.Fields.ContainsKey(name) && _config.Fields[name].Value != null)
            {
                response.AppendFormat("`{0}`'s previous value was `{1}`", name, _config.Fields[name].Value)
                        .AppendLine();
                _config.Fields[name].Value = value;
            }
            else
            {
                _config.Fields[name] = new Field(name, value);
            }

            response.AppendFormat("`{0}` has been set to `{1}`", name, _config.Fields[name].Value).AppendLine();

            return response.ToString();
        }


        public string RollEvents(params string[] objects)
        {
            string response = Help();

            if (objects.Length == 0 || objects[0].ToLower().Equals("help")) return response;

            string choice = objects[0];

            response = "> " + GetResponse(choice, 0);

            return response;
        }

        public string GetResponse(string choice, int depth)
        {
            depth++;

            if (depth > 20)
            {
                return "Too much nesting detected. Check your config file. Aborting!";
            }

            StringBuilder sb = new StringBuilder();

            if (_config.ChoiceMap.ContainsKey(choice.ToLower()))
            {
                sb.AppendLine((string.Format("**{0}**", choice)));
                Choice c = _config.ChoiceMap[choice.ToLower()];

                switch (c.Distribution)
                {
                    case "Weighted":
                        sb.Append(GetWeightedResponse(c, depth));
                        break;
                    case "All":
                        sb.Append(GetAllResponse(c, depth));
                        break;
                    default:
                        sb.AppendLine(String.Format("\"{0}\" has an invalid Distribution", choice));
                        break;
                }

            }
            else
            {
                sb.AppendLine(String.Format("unrecognized parameter **`{0}`**", choice));
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("Something went wrong");
            }
            return sb.ToString();


        }

        // TODO make this a member of Choice
        private StringBuilder GetAllResponse(Choice c, int depth)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Outcome o in c.outcomeMap.Values)
            {

                if (o.ChildChoice == null)
                {
                    sb.AppendLine("\t" + GetOutcomeResponse(o));
                }
                else
                {
                    sb.Append(GetResponse(o.ChildChoice.Name, depth));
                }
            }

            return sb;
        }

        // TODO make this a member of outcome
        private string GetOutcomeResponse(Outcome o)
        {

            string response;

            if (o.Rolls != null && o.Rolls.Length > 0)
            {
                var rolls = new List<int>();
                foreach (string expression in o.Rolls)
                {
                    rolls.Add(_config.Evaluate(expression));
                }

                try
                {
                    response = String.Format(o.Text, rolls.Select(x => x.ToString()).ToArray());
                    response = String.Format("**{0}**", response);
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

        private StringBuilder GetWeightedResponse(Choice c, int depth)
        {
            StringBuilder sb = new StringBuilder();

            int max = c.PossibleOutcomes.Length;
            int roll = DiceRollService.Roll(max) - 1;

            Outcome o = c.outcomeMap[c.PossibleOutcomes[roll]];

            string outcome = GetOutcomeResponse(o);

            if (max <= 1)
            {
                sb.AppendLine(outcome);
            }
            else if (max == 2)
            {
                sb.AppendLine(string.Format("Flipped a coin and got: **{0}**", outcome));
            }
            else
            {
                sb.AppendLine(string.Format("[1d{0}: {1}] **{2}**", max, roll + 1, outcome));
            }

            if (o.ChildChoice != null)
            {
                sb.Append(GetResponse(o.ChildChoice.Name, depth));
            }

            return sb;
        }

        public string GetAllChoices()
        {
            StringBuilder sb = new StringBuilder()
            .AppendLine("> List all possible `!trail` arguments");

            var list = _config.ChoiceMap.Keys.ToList();
            list.Sort();

            foreach (string choice in list)
            {
                sb.AppendLine(String.Format("`{0}`", choice));
            }

            return sb.ToString();
        }

    }
}
