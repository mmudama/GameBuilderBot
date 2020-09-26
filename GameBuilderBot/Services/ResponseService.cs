using Discord.Commands;
using GameBuilderBot.Common;
using GameBuilderBot.Exceptions;
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

        public string HelpForUser()
        {
            try
            {
                var response = new StringBuilder()
                    .AppendLine("> **Help:**");

                foreach (string k in _config.ChoiceMap.Keys)
                {
                    Choice c = _config.ChoiceMap[k];
                    if (c.IsPrimary)
                    {
                        response.AppendLine(String.Format("`!game {0}`: {1}", k, c.Description));
                    }
                }

                return response.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Creates a pretty printed, nested summary of all loaded Choice and Outcome objects.
        /// Sends back a task that either DMs the output directly to the user or, if the
        /// output is too long for a DM, sends a file.
        ///
        /// The output does not represent all object fields and does not conform to any standardized
        /// format - it's just intended to make it easier for a human to review nested Choices.
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task SummarizeEventDataForUser(SocketCommandContext context)
        {
            try
            {
                StringBuilder sbResponse = new StringBuilder();

                foreach (Choice c in _config.ChoiceMap.Values)
                {
                    sbResponse.AppendLine(c.GetSummary());
                }

                string response = sbResponse.ToString();

                if (response.Length < 2000)
                {
                    return context.Channel.SendMessageAsync(response);
                }
                else
                {
                    Stream stream = StreamUtils.GetStreamFromString(response);

                    string fileName = string.Format("{0}_summary.txt", context.Channel.Name);
                    return context.Channel.SendFileAsync(stream, fileName, "The summary is too long to send as a DM; " +
                        "sending it to you as a file instead.");
                }
            }
            catch (Exception e)
            {
                return context.Channel.SendMessageAsync(e.Message);
            }
        }

        internal string DeleteFieldValueForUser(string[] variables)
        {
            try
            {
                if (variables.Length < 1)
                {
                    throw new GameBuilderBotException("Delete requires at least one parameter, e.g. `!delete foo`");
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
            catch (Exception e)
            {
                return e.Message;
            }
        }

        internal Task ExportConfigAsFileForUser(string fileType, SocketCommandContext context)
        {
            try
            {
                if (fileType.Length == 0)
                {
                    return context.Channel.SendMessageAsync("> Missing parameter filetype");
                }

                fileType = fileType.ToUpper();

                string fileContents;
                switch (fileType)
                {
                    case "JSON":
                        fileContents = ExportService.Export(FileType.JSON, _config);
                        break;

                    default:
                        return context.Channel.SendMessageAsync("> Unsupported filetype");
                }

                Stream stream = StreamUtils.GetStreamFromString(fileContents);

                string fileName = string.Format("game_state.txt", context.Channel.Name);
                return context.Channel.SendFileAsync(stream, fileName);
            }
            catch (Exception e)
            {
                return context.Channel.SendMessageAsync(e.Message);
            }
        }

        /// <summary>
        ///
        /// From Discord:
        /// !eval 1d4
        /// !eval 1+1
        /// !eval #mpg#
        /// !eval "#mpg#*3"
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal string EvaluateExpressionForUser(string expression)
        {
            try
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
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// From discord:
        /// !get foo
        /// !get all
        /// !get
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        internal string GetFieldValuesForUser(string[] fieldNames)
        {
            try
            {
                StringBuilder response = new StringBuilder();

                bool getAll = fieldNames.Length == 1 && fieldNames[0].ToLower().Equals("all");

                response.AppendLine("Here you go!");

                if (fieldNames.Length == 0 || getAll)
                {
                    foreach (string key in _config.Fields.Keys)
                    {
                        response.AppendLine(PrettyPrintField(key));
                    }
                }
                else
                {
                    foreach (string s in fieldNames)
                    {
                        response.AppendLine(PrettyPrintField(s));
                    }
                }

                return response.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        protected string PrettyPrintField(string fieldName)
        {
            var response = new StringBuilder();

            string value;
            if (_config.FieldHasValue(fieldName))
            {
                value = _config.Fields[fieldName].Value.ToString();
            }
            else
            {
                value = "**Undefined**";
            }

            response.Append("`")
                .Append(fieldName)
                .Append(": ")
                .AppendFormat("{0}", value);

            if (_config.FieldHasExpression(fieldName))
            {
                response.AppendFormat(" ({0})", _config.Fields[fieldName].Expression);
            }

            response.Append("`");

            return response.ToString();
        }

        internal string SetFieldValueForUser(string[] FieldNameAndValue)
        {
            try
            {
                CalculateFieldValue(FieldNameAndValue, CalculateFieldValueByValue, out string response);
                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
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

        private void CalculateFieldValue(string[] FieldNameAndValue, Func<string, string, int> CalculateValue, out string response)
        {
            try
            {
                if (FieldNameAndValue.Length != 2)
                {
                    throw new GameBuilderBotException("Set takes two arguments. Try `!set foo 100` or `!set foo 1d4`");
                }

                string fieldName = FieldNameAndValue[0].ToLower();
                string expression = FieldNameAndValue[1];
                int value = CalculateValue(fieldName, expression);

                if (int.TryParse(expression, out _))
                {
                    // The second user parameter was an explicit integer value, not an expression
                    expression = null;
                }

                object oldValue = null;

                if (_config.FieldHasValue(fieldName))
                {
                    oldValue = _config.Fields[fieldName].Value;
                }

                if (_config.Fields.ContainsKey(fieldName))
                {
                    _config.Fields[fieldName].Value = value;
                }
                else
                {
                    _config.Fields[fieldName] = new Field(expression, value.ToString());
                }

                response = OutputResponseForCalculateFieldValue(fieldName, oldValue);
            }
            catch (Exception e)
            {
                response = e.Message;
            }
        }

        private string OutputResponseForCalculateFieldValue(string fieldName, object oldValue)
        {
            try
            {
                var sbResponse = new StringBuilder();
                sbResponse.AppendFormat("`{0} = {1}", fieldName, _config.Fields[fieldName].Value);

                if (oldValue != null)
                {
                    sbResponse.AppendFormat(" (was {0})", oldValue);
                }

                return sbResponse.AppendLine("`").ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        internal string SubtractFieldValueForUser(string[] objects)
        {
            try
            {
                CalculateFieldValue(objects, CalculateValueBySubtracting, out string response);
                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
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

        internal string AddFieldValueForUser(string[] objects)
        {
            try
            {
                CalculateFieldValue(objects, CalculateValueByAdding, out string response);
                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
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

        public string RollEventsForUser(params string[] objects)
        {
            try
            {
                string response = HelpForUser();

                if (objects.Length == 0 || objects[0].ToLower().Equals("help")) return response;

                string choice = objects[0];

                response = "> " + GetResponseForEventRoll(choice, 0);

                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string GetResponseForEventRoll(string choice, int depth)
        {
            depth++;

            if (depth > 20)
            {
                return "Too much nesting detected. Check your config file. Aborting!";
            }

            StringBuilder response = new StringBuilder();

            if (_config.ChoiceMap.ContainsKey(choice.ToLower()))
            {
                response.AppendLine((string.Format("**{0}**", choice)));
                Choice c = _config.ChoiceMap[choice.ToLower()];

                switch (c.Distribution)
                {
                    case "Weighted":
                        response.Append(GetResponseForWeightedChoice(c, depth));
                        break;

                    case "All":
                        response.Append(GetResponseForDistributionAllChoice(c, depth));
                        break;

                    default:
                        response.AppendLine(String.Format("\"{0}\" has an invalid Distribution", choice));
                        break;
                }
            }
            else
            {
                response.AppendLine(String.Format("unrecognized parameter **`{0}`**", choice));
            }

            if (response.Length == 0)
            {
                response.AppendLine("Something went wrong");
            }
            return response.ToString();
        }

        // TODO make this a member of Choice
        private StringBuilder GetResponseForDistributionAllChoice(Choice c, int depth)
        {
            StringBuilder response = new StringBuilder();

            foreach (Outcome o in c.outcomeMap.Values)
            {
                if (o.ChildChoice == null)
                {
                    response.AppendLine("\t" + GetResponseForOutcome(o));
                }
                else
                {
                    response.Append(GetResponseForEventRoll(o.ChildChoice.Name, depth));
                }
            }

            return response;
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
            StringBuilder response = new StringBuilder();

            int max = c.PossibleOutcomes.Length;
            int roll = DiceRollService.Roll(max) - 1;

            Outcome o = c.outcomeMap[c.PossibleOutcomes[roll]];

            string outcome = GetResponseForOutcome(o);

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
                response.Append(GetResponseForEventRoll(o.ChildChoice.Name, depth));
            }

            return response;
        }
    }
}
