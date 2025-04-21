using System;
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

        override protected int CalculateValue(GameState state, string fieldName, string expression)
        {
            int value = 0;

            if (state.FieldHasValue(fieldName))
            {
                value = Convert.ToInt32(state.Fields[fieldName].Value.ToString());
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
