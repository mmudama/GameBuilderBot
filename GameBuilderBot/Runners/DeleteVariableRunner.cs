using Discord.Commands;
using GameBuilderBot.Models;
using GameBuilderBot.Services;

namespace GameBuilderBot.Runners
{
    public class DeleteVariableRunner : CommandRunner
    {
        protected string _helpMessage =
            "To delete a variable from the current game, use `!delete <foo>`. " +
            "You can also delete multiple variables using `!delete <foo> <bar>`";

        protected string[] _variables;
        protected ExportService _exportService;

        public DeleteVariableRunner(GameHandlingService gameHandler, ExportService exportService) : base(gameHandler)
        {
            _exportService = exportService;
        }

        public void Delete(string[] variables, SocketCommandContext discordContext)
        {
            TestArgLength(variables, 1);

            GameState state = _gameService.GetGameStateForActiveGame(discordContext.Channel.Id);

            foreach (string key in variables)
            {
                state.Fields.Remove(key);
                _exportService.ExportGameState(state, discordContext);
            }
        }

        public override string OneLinerHelp()
        {
            return "`!delete foo` removes the variable foo from this context";
        }
    }
}
