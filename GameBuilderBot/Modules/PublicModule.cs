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
            => ReplyAsync(ResponseService.Help());



        [Command("game")]
        [Alias("g", "gb", "gamebuilder")]
        public Task TrailAsync(params string[] objects)
        => ReplyAsync(ResponseService.RollEvents(objects));


        [Command("summary")]
        public Task SummarizeAsync() => ResponseService.SummarizeEventData(this.Context);


        [Command("get")]
        [Alias("list")]
        public Task GetAsync(params string[] objects) => ReplyAsync(ResponseService.GetPrettyPrintedFieldValues(objects));

        [Command("set")]
        public Task SetAsync(params string[] objects) => ReplyAsync(ResponseService.SetFieldValue(objects));

        [Command("Delete")]
        [Alias("del", "remove", "rm", "unset")]
        public Task DeleteAsync(params string[] objects) => ReplyAsync(ResponseService.DeleteFieldValue(objects));

        [Command("evaluate")]
        [Alias("eval")]
        public Task EvaluateAsync([Remainder] string expression) => ReplyAsync(ResponseService.EvaluateExpression(expression));

        [Command("add")]
        [Alias("+")]
        public Task AddAsync(params string[] objects) => ReplyAsync(ResponseService.AddFieldValue(objects));

        [Command("subtract")]
        [Alias("sub", "-")]
        public Task SubAsync(params string[] objects) => ReplyAsync(ResponseService.SubtractFieldValue(objects));

        [Command("export")]
        public Task ExportAsync(params string[] objects) => ResponseService.ExportConfigAsFile(objects, this.Context);

    }



}
