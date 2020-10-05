using GameBuilderBot.Models;
using GameBuilderBot.Services;
using System.Text;

namespace GameBuilderBot.Runners
{
    public class PrettyPrintVariableRunner : CommandRunner
    {
        protected string _helpMessage =
            "Type `!get` or `!get all` to see all game variables. Type `!get <name>` to see the value of <name>";

        protected string[] _variables;

        public PrettyPrintVariableRunner(GameHandlingService gameHandler) : base(gameHandler)
        {
        }

        public string PrettyPrint(string[] fieldNames, ulong channelId)
        {
            GameState state = _gameService.GetGameStateForActiveGame(channelId);
            bool getAll = fieldNames.Length == 1 && fieldNames[0].ToLower().Equals("all");
            StringBuilder response = new StringBuilder();

            response.AppendLine("The following variables have been set:");

            if (fieldNames.Length == 0 || getAll)
            {
                if (state != null)
                {
                    foreach (string key in state.Fields.Keys)
                    {
                        response.AppendLine(PrettyPrintField(state, key));
                    }
                }
                else response.AppendLine("No values found");
            }
            else
            {
                foreach (string s in fieldNames)
                {
                    response.AppendLine(PrettyPrintField(state, s));
                }
            }

            return response.ToString();
        }

        protected string PrettyPrintField(GameState state, string fieldName)
        {
            var response = new StringBuilder();

            if (state == null)
            {
                return "Error: could not find game state";
            }

            string value;
            if (state.FieldHasValue(fieldName))
            {
                value = state.Fields[fieldName].Value.ToString();
            }
            else
            {
                value = "**Undefined**";
            }

            response.Append($"`{fieldName}: {value}");

            if (state.FieldHasExpression(fieldName))
            {
                response.AppendFormat($" ({state.Fields[fieldName].Expression})");
            }

            response.Append("`");

            return response.ToString();
        }

        public override string OneLinerHelp()
        {
            return "`!get foo` will return the current value of foo. `!get all` will return all game variables";
        }
    }
}
