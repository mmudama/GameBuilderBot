using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using GameBuilderBot.Services;
using System;

namespace GameBuilderBot.Runners
{
    public class EvaluateExpressionRunner : CommandRunner
    {
        protected string[] _variables;

        public EvaluateExpressionRunner(GameHandlingService gameHandler) : base(gameHandler)
        {
        }

        public int Evaluate(string expression, ulong channelId)
        {
            GameState state = _gameService.GetGameStateForActiveGame(channelId);
            expression = state.ReplaceVariablesWithValues(expression);

            try
            {
                return DiceRollService.Roll(expression);
            }
            catch (Exception)
            {
                throw new GameBuilderBotException(String.Format("Failure attempting to evaluate `{0}`", expression));
            }
        }

        public override string OneLinerHelp()
        {
            return "`!evaluate 1d4` will return the integer value of 1d4";
        }
    }
}
