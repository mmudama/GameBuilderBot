using GameBuilderBot.Services;
using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class Config
    {
        public Dictionary<string, Choice> ChoiceMap;
        public Dictionary<string, Field> Fields;

        public Config(Dictionary<string, Choice> choiceMap, Dictionary<string, Field> fields)
        {
            ChoiceMap = choiceMap;
            Fields = fields;
        }

        /// <summary>
        /// Input can be an expression (like 1d4)
        /// or a reference to a Field. If it starts with "!" and is a recognized
        /// Field key, then reroll. Otherwise use the current value.
        /// If it's null, roll and set the value.
        /// _config.Evaluate("!Sunrise") // found in map, prefix "!" means reroll and set
        /// _config.Evaluate("Sunrise") // found in map, retrieve (or roll if null) // should I allow negatives?
        /// _config.Evaluate("1d4") // not found in map, so evaluate expression
        /// TODO: Might want to use something other than "!" since it's meaningful in yaml
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal int Evaluate(string expression)
        {
            bool evaluate = false;
            string key = expression;
            int result;

            if (expression.StartsWith("!"))
            {
                evaluate = true;
                key = expression.Substring(1);
            }

            if (Fields.ContainsKey(key) && evaluate)
            {
                result = DiceRollService.Roll(Fields[key].Expression);
                Fields[key].Value = result;
            }
            else if (Fields.ContainsKey(key))
            {
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
