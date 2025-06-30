using System;
using System.Collections.Generic;
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

        abstract protected object CalculateValue(GameState state, string fieldName, string expression);


        /// <summary>
        /// Using the inputs from a Discord message, outputs both the previous and new values for the field
        /// </summary>
        /// <param name="FieldNameAndValue">Inputs from Discord message</param>
        /// <param name="discordContext"></param>
        /// <param name="previousValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public void CalculateFieldValue(string[] FieldNameAndValue, SocketCommandContext discordContext, out object previousValue, out object newValue)
        {
            if (FieldNameAndValue.Length != 2)
            {
                throw new GameBuilderBotException(OneLinerHelp());
            }

            GameState state = _gameService.GetGameStateForActiveGame(discordContext.Channel.Id);

            string fieldName = FieldNameAndValue[0].ToLower();
            string expression = FieldNameAndValue[1];
            newValue = CalculateValue(state, fieldName, expression);
            
            PopulateField(state, fieldName, in newValue, expression, out previousValue);

            _exportService.ExportGameState(state, discordContext);
        }

        /// create or populate a Field object within a GameState
        /// 
        /// <returns>true if there was a previous value; else false</returns>
        /// 
        protected bool PopulateField(GameState state, string fieldName, in object newValue, string expression, out object previousValue)
        {
            previousValue = null;

            if (state.FieldHasValue(fieldName))
            {
                previousValue = state.Fields[fieldName].Value;
                state.Fields[fieldName].Value = newValue;

                // this doesn't work for the add / subtract commands. 
                state.Fields[fieldName].Expression = expression;
            }
            else
            {
                state.Fields[fieldName] = new Field(expression, newValue.ToString());
            }

            if (previousValue == null)
            {
                return false;
            }
            else
            {
                return true;
            }


        }
    }
}
