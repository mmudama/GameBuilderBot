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
            MathExpression mathexpression = new(expression, state.Fields);

            if (DiceRollService.TryRoll(expression, out int roll))
            {
                value = roll;
            } 
            else if (DateTime.TryParse(expression, out DateTime dateTimeValue))
            {
                value = dateTimeValue;
            } 
            else if (mathexpression.TryEvaluate(out object result))
            {
                value = result;
            } else
            {
                value = expression;
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
