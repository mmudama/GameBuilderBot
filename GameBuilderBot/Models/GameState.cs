using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class GameState
    {
        public string Name { get; set; }
        public Dictionary<string, Field> Fields { get; set; }
        public ulong ChannelId { get; set; }
        public string FriendlyName { get; set; }

        public bool FieldHasValue(string fieldName)
        {
            return Fields.ContainsKey(fieldName)
                && !(Fields[fieldName] == null)
                && !(Fields[fieldName].Value == null);
        }

        public bool FieldHasExpression(string fieldName)
        {
            return Fields.ContainsKey(fieldName)
                && !(Fields[fieldName] == null)
                && !(Fields[fieldName].Expression == null);
        }

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
