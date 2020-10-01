using GameBuilderBot.Services;
using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// GameDefinition provides the structure of a game. It determines the parameters available
    /// via `!game`, what the possible outcomes of that call will be, and their probabilities.
    /// See the Example directory for ideas.
    /// </summary>
    /// <seealso cref="GameFile"/>
    public class GameDefinition
    {
        /// <summary>
        /// "Friendly" name to identify the game, like "The Quest for the Holy Grail" or
        /// "Robot Hijinx." This will be exposed directly in chat.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contains all available <seealso cref="Choice"/> objects for this game.
        /// </summary>
        public Dictionary<string, Choice> ChoiceMap;

        /// <summary>
        /// Constructor that populates all members
        /// </summary>
        /// <param name="name"></param>
        /// <param name="choiceMap"></param>
        public GameDefinition(string name, Dictionary<string, Choice> choiceMap)
        {
            ChoiceMap = choiceMap;
            Name = name;
        }

        // TODO this function doesn't belong here and needs to be rethought and renamed
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
    }
}
