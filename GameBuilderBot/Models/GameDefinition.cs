using GameBuilderBot.Services;
using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class GameDefinition
    {
        /// <summary>
        /// User-provided name to identify game. Allows the user to switch back and forth among
        /// different games in the same channel.
        /// </summary>
        public string Name { get; set; }

        public Dictionary<string, Choice> ChoiceMap;
        public Dictionary<string, Field> Fields { get; set; }

        public GameDefinition(string name, Dictionary<string, Choice> choiceMap, Dictionary<string, Field> fields)
        {
            ChoiceMap = choiceMap;
            Fields = fields;
            Name = name;
        }

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

        // TODO put CalculateExpressionAndSometimesSetFieldValue somewhere else? Maybe have a class just for parsing
        // TODO definitely break it up and make it make more sense

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

        internal static int CalculateExpressionAndSometimesSetFieldValue(string expression, Dictionary<string, Field> fields)
        {
            bool shouldCalculate = false;
            string key = expression;
            int result;

            if (expression.StartsWith("!"))
            {
                shouldCalculate = true;
                key = expression.Substring(1).ToLower();
            }

            if (fields.ContainsKey(key) && shouldCalculate)
            {
                key = key.ToLower();
                result = DiceRollService.Roll(fields[key].Expression);
                fields[key].Value = result;
            }
            else if (fields.ContainsKey(key))
            {
                key = key.ToLower();
                if (fields[key].Value == null)
                {
                    fields[key].Value = DiceRollService.Roll(fields[key].Expression);
                }
                result = (int)fields[key].Value;
            }
            else
            {
                // assume it's a dice roll expression e.g. "1d6+2"
                result = DiceRollService.Roll(key);
            }

            return result;
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
