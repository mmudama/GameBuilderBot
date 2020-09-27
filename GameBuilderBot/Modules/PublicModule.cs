using Discord.Commands;
using GameBuilderBot.Services;
using System.Threading.Tasks;

namespace GameBuilderBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        protected ResponseService ResponseService;

        public PublicModule(ResponseService responseService)
        {
            ResponseService = responseService;
        }

        [Command("help")]
        public Task HelpAsync()
            => ReplyAsync(ResponseService.HelpForUser());

        [Command("game")]
        [Alias("g", "gb", "gamebuilder")]
        public Task RollEventsAsync(params string[] inputs)
        => ReplyAsync(ResponseService.RollEventsForUser(inputs));

        [Command("summary")]
        public Task SummarizeAsync() => ResponseService.SummarizeEventDataForUser(this.Context);

        [Command("get")]
        [Alias("list")]
        public Task GetAsync(params string[] inputs) => ReplyAsync(ResponseService.GetFieldValuesForUser(inputs));

        [Command("set")]
        public Task SetAsync(params string[] inputs) => ReplyAsync(ResponseService.SetFieldValueForUser(inputs));

        [Command("Delete")]
        [Alias("del", "remove", "rm", "unset")]
        public Task DeleteAsync(params string[] inputs) => ReplyAsync(ResponseService.DeleteFieldValueForUser(inputs));

        [Command("evaluate")]
        [Alias("eval")]
        public Task EvaluateAsync([Remainder] string expression) => ReplyAsync(ResponseService.EvaluateExpressionForUser(expression));

        [Command("add")]
        [Alias("+")]
        public Task AddAsync(params string[] inputs) => ReplyAsync(ResponseService.AddFieldValueForUser(inputs));

        [Command("subtract")]
        [Alias("sub", "-")]
        public Task SubAsync(params string[] inputs) => ReplyAsync(ResponseService.SubtractFieldValueForUser(inputs));

        [Command("export")]
        public Task ExportAsync([Remainder] string fileType) => ResponseService.ExportConfigAsFileForUser(fileType, Context);
    }
}
