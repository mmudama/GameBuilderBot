using Discord.Commands;
using GameBuilderBot.ExpressionHandling;
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
        private readonly GameConfig _config;

        public ResponseService(GameConfig config)
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


        public Task SummarizeEventData(SocketCommandContext context)
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

        internal string DeleteFieldValue(string[] variables)
        {
            var errorResponse = "> Delete syntax: !delete <name> (<name> <name> ...)";

            if (variables.Length < 1)
            {
                return errorResponse;
            }

            foreach (string key in variables)
            {
                if (_config.Fields.ContainsKey(key))
                {
                    _config.Fields.Remove(key);
                }
            }

            return String.Format("Deleted variables: {0}", String.Join(", ", variables));
        }

        internal Task ExportConfigAsFile(string[] objects, SocketCommandContext context)
        {
            if (objects.Length < 1)
            {
                return context.Channel.SendMessageAsync("> Missing parameter filetype");
            }

            string type = objects[0].ToUpper();

            string fileContents = "";

            switch (type)
            {
                case "JSON":
                    fileContents = ExportService.Export(FileType.JSON, _config);
                    break;
                default:
                    return context.Channel.SendMessageAsync("> Unsupported filetype");
            }

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(fileContents);
            writer.Flush();
            stream.Position = 0;

            string fileName = string.Format("game_state.txt", context.Channel.Name);
            return context.Channel.SendFileAsync(stream, fileName);
        }

        internal string EvaluateExpression(string expression)
        {
            expression = _config.ReplaceVariablesWithValues(expression);

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

        internal string GetPrettyPrintedFieldValues(string[] objects)
        {
            StringBuilder sb = new StringBuilder();

            bool getAll = objects.Length == 1 && objects[0].ToLower().Equals("all");

            if (objects.Length == 0 || getAll)
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

        internal string SetFieldValue(string[] objects)
        {
            CalculateFieldValue(objects, CalculateFieldValueByValue, out string response);
            return response;

        }

        private int CalculateFieldValueByValue(string fieldName, string expression)
        {
            if (!int.TryParse(expression, out int result))
            {
                expression = _config.ReplaceVariablesWithValues(expression);
                result = DiceRollService.Roll(expression);
            }

            return result;
        }

        private void CalculateFieldValue(string[] objects, Func<string, string, int> CalculateValue, out string response)
        {
            var errorResponse = "> set syntax: `!set <name> <integer value>`" +
            "\n OR `!set <name> <expression>` (like 1d4 or 1+5)";

            try
            {
                if (objects.Length != 2)
                {
                    response = errorResponse;
                    return;
                }

                string name = objects[0].ToLower();
                string expression = objects[1];
                int value = CalculateValue(name, expression);

                if (int.TryParse(expression, out _))
                {
                    // The second user parameter was an explicit integer value, not an expression
                    expression = null;
                }

                object oldValue = null;

                if (_config.FieldHasValue(name))
                {
                    oldValue = _config.Fields[name].Value;
                }
                
                if (_config.Fields.ContainsKey(name))
                {
                    _config.Fields[name].Value = value;

                    // TODO should this also set the expression if it exists?
                    // Punt because Aaron wants to get rid of all that anyway
                }
                else
                {
                    _config.Fields[name] = new Field(expression, value.ToString());
                }

                response = OutputResponseForCalculateFieldValue(name, _config.Fields[name].Value, oldValue);
            }
            catch (Exception)
            {
                response = errorResponse;
                return;
            }
        }

        private string OutputResponseForCalculateFieldValue(string name, object value, object oldValue)
        {

            var sbResponse = new StringBuilder();
            sbResponse.AppendFormat("`{0} = {1}", name, _config.Fields[name].Value);

            if (oldValue != null)
            {
                sbResponse.AppendFormat(" (was {0})", oldValue);
            }

            return sbResponse.AppendLine("`").ToString();
        }

        internal string SubtractFieldValue(string[] objects)
        {
            CalculateFieldValue(objects, CalculateValueBySubtracting, out string response);
            return response;
        }

        private int CalculateValueBySubtracting(string fieldName, string expression)
        {
            int value = 0;

            if (_config.FieldHasValue(fieldName))
            {
                value = (int)_config.Fields[fieldName].Value;
            }

            expression = _config.ReplaceVariablesWithValues(expression);
            return value - DiceRollService.Roll(expression);
        }

        internal string AddFieldValue(string[] objects)
        {
            CalculateFieldValue(objects, CalculateValueByAdding, out string response);
            return response;
        }

        private int CalculateValueByAdding(string fieldName, string expression)
        {
            int value = 0;

            if (_config.FieldHasValue(fieldName))
            {
                value = Convert.ToInt32(_config.Fields[fieldName].Value.ToString());
            }

            MathExpression mathexpression = new MathExpression(expression, _config.Fields);
            return value + Convert.ToInt32(mathexpression.Evaluate().ToString());
        }

        public string RollEvents(params string[] objects)
        {
            string response = Help();

            if (objects.Length == 0 || objects[0].ToLower().Equals("help")) return response;

            string choice = objects[0];

            response = "> " + GetResponseForEventRoll(choice, 0);

            return response;
        }

        public string GetResponseForEventRoll(string choice, int depth)
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
                        sb.Append(GetResponseForWeightedChoice(c, depth));
                        break;
                    case "All":
                        sb.Append(GetResponseForDistributionAllChoice(c, depth));
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
        private StringBuilder GetResponseForDistributionAllChoice(Choice c, int depth)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Outcome o in c.outcomeMap.Values)
            {

                if (o.ChildChoice == null)
                {
                    sb.AppendLine("\t" + GetResponseForOutcome(o));
                }
                else
                {
                    sb.Append(GetResponseForEventRoll(o.ChildChoice.Name, depth));
                }
            }

            return sb;
        }

        // TODO make this a member of outcome
        private string GetResponseForOutcome(Outcome o)
        {

            string response;

            if (o.Rolls != null && o.Rolls.Length > 0)
            {
                var rolls = new List<int>();
                foreach (string expression in o.Rolls)
                {
                    rolls.Add(GameConfig.CalculateExpressionAndSometimesSetFieldValue(expression, _config.Fields));
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

        // TODO make this a member of choice
        private StringBuilder GetResponseForWeightedChoice(Choice c, int depth)
        {
            StringBuilder sb = new StringBuilder();

            int max = c.PossibleOutcomes.Length;
            int roll = DiceRollService.Roll(max) - 1;

            Outcome o = c.outcomeMap[c.PossibleOutcomes[roll]];

            string outcome = GetResponseForOutcome(o);

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
                sb.Append(GetResponseForEventRoll(o.ChildChoice.Name, depth));
            }

            return sb;
        }

    }
}
