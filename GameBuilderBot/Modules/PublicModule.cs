using Discord.Commands;
using GameBuilderBot.Services;
using System.Threading.Tasks;

namespace GameBuilderBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        protected ResponseService _responseService;
        protected GameHandlingService _gameHandlingService;

        public PublicModule(ResponseService responseService, GameHandlingService gameHandlingService)
        {
            _responseService = responseService;
            _gameHandlingService = gameHandlingService;
        }

        [Command("help")]
        public Task HelpAsync()
            => ReplyAsync(_responseService.HelpForUser());

        [Command("Start")]
        public Task StartGameAsync(params string[] inputs) => ReplyAsync(_responseService.StartGame(inputs, Context));

        [Command("Restore")]
        public Task RestoreGameAsync() => ReplyAsync(_responseService.RestoreGame(Context.Channel.Id));

        [Command("game")]
        [Alias("g", "gb", "gamebuilder")]
        public Task RollEventsAsync(params string[] inputs)
        => ReplyAsync(_responseService.RollEventsForUser(Context, inputs));

        [Command("get")]
        [Alias("list")]
        public Task GetAsync(params string[] inputs) => ReplyAsync(_responseService.GetFieldValuesForUser(Context.Channel.Id, inputs));

        [Command("set")]
        public Task SetAsync(params string[] inputs) => _responseService.SetFieldValueForUser(inputs, Context);

        [Command("Delete")]
        [Alias("del", "remove", "rm", "unset")]
        public Task DeleteAsync(params string[] inputs) => ReplyAsync(_responseService.DeleteFieldValueForUser(inputs, Context));

        [Command("evaluate")]
        [Alias("eval")]
        public Task EvaluateAsync([Remainder] string expression) => ReplyAsync(_responseService.EvaluateExpressionForUser(expression, Context.Channel.Id));

        [Command("add")]
        public Task AddAsync(params string[] inputs) => _responseService.AddFieldValueForUser(inputs, Context);

        [Command("subtract")]
        public Task SubAsync(params string[] inputs) => _responseService.SubtractFieldValueForUser(inputs, Context);
    }
}
