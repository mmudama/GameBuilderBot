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
        /// TODO definitely rename this. Maybe also switch to using out params instead of returning a tuple?
        /// </summary>
        /// <param name="FieldNameAndValue"></param>
        /// <param name="discordContext"></param>
        /// <returns></returns>
        public (object, object) CalculateFieldValue(string[] FieldNameAndValue, SocketCommandContext discordContext)
        {
            try
            {
                if (FieldNameAndValue.Length != 2)
                {
                    throw new GameBuilderBotException(OneLinerHelp());
                }

                GameState state = _gameService.GetGameStateForActiveGame(discordContext.Channel.Id);

                string fieldName = FieldNameAndValue[0].ToLower();
                string expression = FieldNameAndValue[1];
                int value = CalculateValue(state, fieldName, expression);

                if (int.TryParse(expression, out _))
                {
                    // The second user parameter was an explicit integer value, not an expression
                    expression = null;
                }

                object oldValue = null;

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

                _exportService.ExportGameState(state, discordContext);
                return (oldValue, state.Fields[fieldName].Value);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
