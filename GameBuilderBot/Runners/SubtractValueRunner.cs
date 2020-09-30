using GameBuilderBot.ExpressionHandling;
using GameBuilderBot.Models;
using GameBuilderBot.Services;
using System;

namespace GameBuilderBot.Runners
{
    public class SubtractValueRunner : AssignmentRunner
    {
        public SubtractValueRunner(GameHandlingService gameHandler, ExportService exportService) : base(gameHandler, exportService)
        {
        }

        override protected int CalculateValue(GameState state, string fieldName, string expression)
        {
            int value = 0;

            if (state.FieldHasValue(fieldName))
            {
                value = Convert.ToInt32(state.Fields[fieldName].Value.ToString());
            }

            MathExpression mathexpression = new MathExpression(expression, state.Fields);
            return value - Convert.ToInt32(mathexpression.Evaluate().ToString());
        }

        public override string OneLinerHelp()
        {
            return "`!subtract foo 12` subtracts 12 from the variable foo";
        }
    }
}
