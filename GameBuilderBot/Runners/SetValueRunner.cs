using System;
using GameBuilderBot.ExpressionHandling;
using GameBuilderBot.Models;
using GameBuilderBot.Services;

namespace GameBuilderBot.Runners
{
    public class SetValueRunner : AssignmentRunner
    {
        public SetValueRunner(GameHandlingService gameHandler, ExportService exportService) : base(gameHandler, exportService)
        {
        }

        override protected object CalculateValue(GameState state, string fieldName, string expression)
        {


            // Return the value based on the expression:
            // First try the expression as a dice roll (or integer)
            // Then as a DateTime
            // Then as a "math expression," which can also add dates and concatenate strings
            // If all else fails, return the expression as a string


            object value;

            try
            {
            // DiceRollService will also handle integers
                value = DiceRollService.Roll(expression);
            }
            catch (Dice.DiceException) // It wasn't a dice roll
            {
                try
                {
                    value = DateTime.Parse(expression);
                }
                catch (FormatException)
                {
                    try
                    {
                        // It's "math" but it's not numeric, so can't assume int
                        MathExpression mathexpression = new MathExpression(expression, state.Fields);
                        value = mathexpression.Evaluate();
                    }
                    catch (Exception)
                    {
                        value = expression;
                    }
                }
            }

            return value;
        }

        public override string OneLinerHelp()
        {
            // TODO this should be more descriptive
            return "`!set foo 1` sets the variable named foo to the value 1 in the current game";
        }
    }
}
