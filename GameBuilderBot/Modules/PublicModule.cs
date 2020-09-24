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
        [Alias("g", "gb")]
        public Task TrailAsync(params string[] objects)
        => ReplyAsync(ResponseService.RollEvents(objects));


        [Command("summary")]
        public Task SummarizeAsync() => ResponseService.Summarize(this.Context);

        [Command("list")]
        public Task ListAsync() => ReplyAsync(ResponseService.GetAllChoices());

        [Command("values")]
        public Task ValuesAsync(params string[] objects) => ReplyAsync(ResponseService.Values(objects));

        [Command("set")]
        public Task SetAsync(params string[] objects) => ReplyAsync(ResponseService.Set(objects));

    }



}
