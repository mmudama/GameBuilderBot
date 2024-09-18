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

        override protected int CalculateValue(GameState state, string fieldName, string expression)
        {
            if (!int.TryParse(expression, out int result))
            {
                MathExpression mathexpression = new MathExpression(expression, state.Fields);
                result = Convert.ToInt32(mathexpression.Evaluate().ToString());
            }

            return result;
        }

        public override string OneLinerHelp()
        {
            return "`!set foo 1` sets the variable named foo to the value 1 in the current game";
        }
    }
}
