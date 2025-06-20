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
            // First try to treat it as an integer
            // If it's not an integer, try to treat it as a dice roll / mathematical expression
            // Then try a datetime
            // If that fails, just return the expression (string)

            object value;
            if (!int.TryParse(expression, out int number))
            {
                try
                {
                    MathExpression mathexpression = new MathExpression(expression, state.Fields);
                    value = Convert.ToInt32(mathexpression.Evaluate().ToString());
                }
                catch (Dice.DiceException) // It wasn't a dice roll
                {
                    try
                    {
                        value = DateTime.Parse(expression);
                    }
                    catch (FormatException) // It wasn't a DateTime
                    {
                        value = expression;
                    }
                }

            }
            else
            {
                value = number;
            }

            return value;
        }

        public override string OneLinerHelp()
        {
            return "`!set foo 1` sets the variable named foo to the value 1 in the current game";
        }
    }
}
