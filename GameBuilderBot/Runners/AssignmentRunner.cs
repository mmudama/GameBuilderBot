using System;
using Discord.Commands;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using GameBuilderBot.Services;

namespace GameBuilderBot.Runners
{
    internal enum Operation
    {
        SET,
        ADD,
        SUBTRACT
    }

    /// <summary>
    /// For bot commands that set the value of a name/value pair.
    /// TODO can this be simplified since add and delete are really just 
    /// sopecial cases of set?
    /// </summary>
    abstract public class AssignmentRunner : CommandRunner
    {
        private readonly ExportService _exportService;

        public AssignmentRunner(GameHandlingService gameHandler, ExportService exportService) : base(gameHandler)
        {
            _exportService = exportService;
        }

        abstract protected int CalculateValue(GameState state, string fieldName, string expression);

        /// <summary>
        /// TODO What actually is the summary?
        /// </summary>
        /// <param name="FieldNameAndValue"></param>
        /// <param name="discordContext"></param>
        /// <returns></returns>
        public void CalculateFieldValue(string[] FieldNameAndValue, SocketCommandContext discordContext, out object oldValue, out object newValue)
        {
            if (FieldNameAndValue.Length != 2)
            {
                throw new GameBuilderBotException(OneLinerHelp());
            }

            GameState state = _gameService.GetGameStateForActiveGame(discordContext.Channel.Id);

            string fieldName = FieldNameAndValue[0].ToLower();
            string expression = FieldNameAndValue[1];
            int value = CalculateValue(state, fieldName, expression);

            oldValue = null;

            if (int.TryParse(expression, out _))
            {
                // The second user parameter was an explicit integer value, not an expression
                expression = null;
            }
            
            if (state.FieldHasValue(fieldName))
            {
                oldValue = state.Fields[fieldName].Value;
            }

            if (state.Fields.ContainsKey(fieldName))
            {
                state.Fields[fieldName].Value = value;
            }
            else
            {
                state.Fields[fieldName] = new Field(expression, value.ToString());
            }

            newValue = state.Fields[fieldName].Value;
            _exportService.ExportGameState(state, discordContext);
        }
    }
}
