using GameBuilderBot.Services;
using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// GameState stores per-channel, per-game variables
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Used as part of the key for storage and retrieval
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name/Value pairs as set by the game or by users directly
        /// </summary>
        public Dictionary<string, Field> Fields { get; set; }

        /// <summary>
        /// Unique permanent identifier provided by Discord for each "channel."
        /// For our purposes, channels are either DM conversations with a particular user,
        /// or they are a particular channel within a particular server.
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// This is included in the GameState export for ease of trouble shooting -
        /// it represents the discord server/channel combo OR the other username
        /// in a DM conversation, but is not recommended for programmatic use.
        /// </summary>
        public string FriendlyName { get; set; }

        // TODO Update convenience methods below with an "out" parameter much like TryParse

        /// <summary>
        /// Convenience method to wrap multiple null checks before retrieving Value
        /// </summary>
        /// <param name="fieldName">The field whose value we're checking</param>
        /// <returns></returns>
        public bool FieldHasValue(string fieldName)
        {
            return Fields.ContainsKey(fieldName)
                && !(Fields[fieldName] == null)
                && !(Fields[fieldName].Value == null);
        }

        /// <summary>
        /// Convenience method to wrap multiple null checks before retrieving Expression
        /// </summary>
        /// <param name="fieldName">The field whose expression we're checking</param>
        /// <returns></returns>
        public bool FieldHasExpression(string fieldName)
        {
            return Fields.ContainsKey(fieldName)
                && !(Fields[fieldName] == null)
                && !(Fields[fieldName].Expression == null);
        }

        // TODO move ReplaceVariablesWithValues to wherever it belongs
        /// <summary>
        /// If an expression string contains # characters, attempt to replace
        /// the section(s) surrounded by # signs with Field contents. For example,
        /// "#gallons# * #mpg#" might be returned as "100 * 3"
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string ReplaceVariablesWithValues(string expression)
        {
            string[] parts = expression.Split('#');

            for (int i = 0; i < parts.Length; i++)
            {
                if (Fields.ContainsKey(parts[i]))
                {
                    parts[i] = Fields[parts[i]].Value.ToString();
                }
            }

            expression = string.Join(" ", parts);
            return expression;
        }

        /// <summary>
        /// Input can be an expression (like 1d4)
        /// or a reference to a Field. If it starts with "!" and is a recognized
        /// Field key, then reroll. Otherwise use the current value.
        /// If it's null, roll and set the value.
        /// _gameDefinition.Evaluate("!Sunrise") // found in map, prefix "!" means reroll and set
        /// _gameDefinition.Evaluate("Sunrise") // found in map, retrieve (or roll if null) // should I allow negatives?
        /// _gameDefinition.Evaluate("1d4") // not found in map, so evaluate expression
        /// TODO: Might want to use something other than "!" since it's meaningful in yaml
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal int SetFieldValueByExpression(string expression)
        {
            bool shouldCalculate = false;
            string key = expression;
            int result;

            if (expression.StartsWith("!"))
            {
                shouldCalculate = true;
                key = expression.Substring(1).ToLower();
            }

            if (Fields.ContainsKey(key) && shouldCalculate)
            {
                key = key.ToLower();
                result = DiceRollService.Roll(Fields[key].Expression);
                Fields[key].Value = result;
            }
            else if (Fields.ContainsKey(key))
            {
                key = key.ToLower();
                if (Fields[key].Value == null)
                {
                    Fields[key].Value = DiceRollService.Roll(Fields[key].Expression);
                }
                result = (int)Fields[key].Value;
            }
            else
            {
                // assume it's a dice roll expression e.g. "1d6+2"
                result = DiceRollService.Roll(key);
            }

            return result;
        }
    }
}
