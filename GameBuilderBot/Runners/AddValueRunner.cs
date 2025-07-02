using System;
using System.Numerics;
using GameBuilderBot.Exceptions;
using GameBuilderBot.ExpressionHandling;
using GameBuilderBot.Models;
using GameBuilderBot.Services;

namespace GameBuilderBot.Runners
{
    public class AddValueRunner : AssignmentRunner
    {
        public AddValueRunner(GameHandlingService gameHandler, ExportService exportService) : base(gameHandler, exportService)
        {
        }

        override public object CalculateValue(GameState state, string fieldName, string expression)
        {
            int value = 0;

            if (state.FieldHasValue(fieldName))
            {
                if (!Int32.TryParse(state.Fields[fieldName].Value.ToString(), out value))
                {
                    throw new GameBuilderBotException($"The `add` command only supports integers");
                }

            }

            MathExpression mathexpression = new(expression, state.Fields);
            return value + Convert.ToInt32(mathexpression.Evaluate().ToString());
        }

        public override string OneLinerHelp()
        {
            return "`!add foo 12` adds 12 to the variable foo in the current game";
        }
    }
}
