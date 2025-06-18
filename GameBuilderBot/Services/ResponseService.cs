using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using GameBuilderBot.Runners;
using Microsoft.Extensions.DependencyInjection;

namespace GameBuilderBot.Services
{
    /// <summary>
    /// Method calls for requests received via Discord. Methods that represent top-level calls
    /// from Discord chat should by convention have the suffix "ForUser"
    /// </summary>
    public class ResponseService
    {
        private readonly GameHandlingService _gameService;
        private readonly ExportService _exportService;
        private readonly DeleteVariableRunner _deleteVariableRunner;
        private readonly EvaluateExpressionRunner _evaluateExpressionRunner;
        private readonly PrettyPrintVariableRunner _prettyPrintVariableRunner;
        private readonly RollEventRunner _rollEventRunner;
        private readonly StartGameRunner _startGameRunner;
        private readonly EndGameRunner _endGameRunner;
        private readonly SetValueRunner _setValueRunner;
        private readonly AddValueRunner _addValueRunner;
        private readonly SubtractValueRunner _subtractValueRunner;

        private readonly List<CommandRunner> _runners = [];

        private const string NO_ACTIVE_GAME_RESPONSE = "No active game found. Type `!start` to start a game.";

        public ResponseService(IServiceProvider services)
        {
            _gameService = services.GetRequiredService<GameHandlingService>();
            _exportService = services.GetRequiredService<ExportService>();

            // Registration order of runners dictates order of output in the help message
            _startGameRunner = RegisterRunner(new StartGameRunner(_gameService));
            _endGameRunner = RegisterRunner(new EndGameRunner(_gameService));
            _rollEventRunner = RegisterRunner(new RollEventRunner(_gameService));
            _deleteVariableRunner = RegisterRunner(new DeleteVariableRunner(_gameService, _exportService));
            _prettyPrintVariableRunner = RegisterRunner(new PrettyPrintVariableRunner(_gameService));
            _setValueRunner = RegisterRunner(new SetValueRunner(_gameService, _exportService));
            _subtractValueRunner = RegisterRunner(new SubtractValueRunner(_gameService, _exportService));
            _addValueRunner = RegisterRunner(new AddValueRunner(_gameService, _exportService));
            _evaluateExpressionRunner = RegisterRunner(new EvaluateExpressionRunner(_gameService));
        }

        private T RegisterRunner<T>(T runner) where T : CommandRunner
        {
            _runners.Add(runner);
            return runner;
        }

        public string HelpForUser()
        {
            StringBuilder sbResponse = new StringBuilder();

            sbResponse.AppendLine("Welcome to the Game Builder Bot!");

            foreach (CommandRunner runner in _runners)
            {
                sbResponse.AppendLine(runner.OneLinerHelp());
            }

            return sbResponse.ToString();
        }

        /// <summary>
        /// 
        /// If <paramref name="inputs"/> is empty, StartGame will give the user a list
        /// of games from which they may choose. If inputs contains at least one value,
        /// StartGame will attempt to treat the value as an integer representing
        /// a game selection.
        /// 
        /// Note: If the bot's game definitions are reloaded between a `!start` call
        /// and a `!start N` call, it is possible that the second command will load
        /// the wrong game. This could potentially also become an issue if multiple
        /// GameBuilderBot instances are linked to the same Discord bot.
        /// 
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        internal string StartGame(string[] inputs, SocketCommandContext context)
        {
            StringBuilder sbResponse = new StringBuilder();
            try
            {
                if (_startGameRunner.DisplayAllGames(inputs, out List<GameDefinition> definitions))
                {
                    sbResponse.AppendLine("To start a game:");

                    for (int i = 0; i < definitions.Count; i++)
                    {
                        sbResponse.AppendFormat("`!start {0}` for ", i + 1);
                        sbResponse.AppendLine(definitions[i].Name);
                    }
                }
                else
                {
                    ulong channelId = context.Channel.Id;

                    string gameName = _startGameRunner.StartGame(inputs, channelId);

                    sbResponse.AppendLine($"Loaded game {gameName}");

                    if (_startGameRunner.RestoreGame(channelId))
                    {
                        sbResponse.AppendLine("Your game settings have been restored");
                        sbResponse.Append(GetFieldValuesForUser(channelId, ["all"]));
                    }
                    else
                    {
                        sbResponse.AppendLine("No previous game settings found for this channel; using defaults");
                    }
                    sbResponse.AppendLine($"To see the commands for {gameName}, type `!game help`");
                }
            }
            catch (NoActiveGameException)
            {
                sbResponse = new StringBuilder(NO_ACTIVE_GAME_RESPONSE);
            }
            catch (Exception e)
            {
                sbResponse = new StringBuilder(e.Message);
            }
            return sbResponse.ToString();
        }

        internal string EndGame(string[] inputs, SocketCommandContext context)
        {
            string gameName = _endGameRunner.EndGame(context.Channel.Id);
            return string.Format("The game '{0}' has ended", gameName);
        }

        internal string DeleteFieldValueForUser(string[] variables, SocketCommandContext discordContext)
        {
            try
            {
                _deleteVariableRunner.Delete(variables, discordContext);
                return string.Format("Deleted variables: {0}", string.Join(", ", variables));
            }
            catch (NoActiveGameException)
            {
                return NO_ACTIVE_GAME_RESPONSE;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        ///
        /// From Discord:
        /// !eval 1d4
        /// !eval 1+1
        /// !eval #mpg#
        /// !eval "#mpg#*3"
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal string EvaluateExpressionForUser(string expression, ulong channelId)
        {
            try
            {
                int value = _evaluateExpressionRunner.Evaluate(expression, channelId);
                return string.Format("`{0} = {1}`", expression, value);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// From discord:
        /// !get foo
        /// !get foo bar baz
        /// !get all
        /// !get
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        internal string GetFieldValuesForUser(ulong channelId, string[] fieldNames)
        {
            try
            {
                return _prettyPrintVariableRunner.PrettyPrint(fieldNames, channelId);
            }
            catch (NoActiveGameException)
            {
                return NO_ACTIVE_GAME_RESPONSE;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Assign value to key
        /// </summary>
        /// <param name="FieldNameAndValue"></param>
        /// <param name="discordContext"></param>
        /// <returns></returns>
        internal Task SetFieldValueForUser(string[] FieldNameAndValue, SocketCommandContext discordContext)
        {
            try
            {
                (object oldValue, object newValue) = _setValueRunner.CalculateFieldValue(FieldNameAndValue, discordContext);
                string response = AssignmentResponse(FieldNameAndValue[0], oldValue, newValue);
                return discordContext.Channel.SendMessageAsync(response);
            }
            catch (NoActiveGameException)
            {
                return discordContext.Channel.SendMessageAsync(NO_ACTIVE_GAME_RESPONSE);
            }
            catch (Exception e)
            {
                return discordContext.Channel.SendMessageAsync(e.Message);
            }
        }

        private string AssignmentResponse(string fieldName, object oldValue, object newValue)
        {
            try
            {
                var sbResponse = new StringBuilder();
                sbResponse.AppendFormat("`{0} = {1}", fieldName, newValue);

                if (oldValue != null)
                {
                    sbResponse.AppendFormat(" (was {0})", oldValue);
                }

                return sbResponse.AppendLine("`").ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        internal Task SubtractFieldValueForUser(string[] FieldNameAndValue, SocketCommandContext discordContext)
        {
            try
            {
                (object oldValue, object newValue) = _subtractValueRunner.CalculateFieldValue(FieldNameAndValue, discordContext);
                string response = AssignmentResponse(FieldNameAndValue[0], oldValue, newValue);
                return discordContext.Channel.SendMessageAsync(response);
            }
            catch (NoActiveGameException)
            {
                return discordContext.Channel.SendMessageAsync(NO_ACTIVE_GAME_RESPONSE);
            }
            catch (Exception e)
            {
                return discordContext.Channel.SendMessageAsync(e.Message);
            }
        }

        internal Task AddFieldValueForUser(string[] FieldNameAndValue, SocketCommandContext discordContext)
        {
            try
            {
                (object oldValue, object newValue) = _addValueRunner.CalculateFieldValue(FieldNameAndValue, discordContext);
                string response = AssignmentResponse(FieldNameAndValue[0], oldValue, newValue);
                return discordContext.Channel.SendMessageAsync(response);
            }
            catch (NoActiveGameException)
            {
                return discordContext.Channel.SendMessageAsync(NO_ACTIVE_GAME_RESPONSE);
            }
            catch (Exception e)
            {
                return discordContext.Channel.SendMessageAsync(e.Message);
            }
        }

        public string RollEventsForUser(SocketCommandContext discordContext, params string[] events)
        {
            try
            {
                return _rollEventRunner.RollEvent(events, discordContext);
            }
            catch (NoActiveGameException)
            {
                return NO_ACTIVE_GAME_RESPONSE;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
