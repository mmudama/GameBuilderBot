using GameBuilderBot.Services;

namespace GameBuilderBot.Runners
{
    public class RestoreGameRunner : CommandRunner
    {
        protected string _helpMessage =
            "To delete a variable from the current game, use `!delete <foo>`. " +
            "You can also delete multiple variables using `!delete <foo> <bar>`";

        protected string[] _variables;

        public RestoreGameRunner(GameHandlingService gameHandler) : base(gameHandler)
        {
        }

        public bool RestoreGame(ulong channelId)
        {
            _gameService.LoadGameState(channelId, out bool fileFound);
            return fileFound;
        }

        // TODO get rid of this and just load by default
        public override string OneLinerHelp()
        {
            return "`!restore` will load your old game variables";
        }
    }
}
