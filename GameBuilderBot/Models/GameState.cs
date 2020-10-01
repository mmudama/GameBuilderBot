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
    }
}
