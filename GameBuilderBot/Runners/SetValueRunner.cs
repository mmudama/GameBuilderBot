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
                expression = state.ReplaceVariablesWithValues(expression);
                result = DiceRollService.Roll(expression);
            }

            return result;
        }

        public override string OneLinerHelp()
        {
            return "`!set foo 1` sets the variable named foo to the value 1 in the current game";
        }
    }
}
