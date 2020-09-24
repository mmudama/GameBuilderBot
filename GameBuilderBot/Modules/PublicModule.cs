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

        [Command("ping")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("help")]
        public Task HelpAsync()
            => ReplyAsync(ResponseService.Help());



        [Command("game")]
        public Task TrailAsync(params string[] objects)
        => ReplyAsync(ResponseService.RollEvents(objects));


        [Command("summary")]
        public Task SummarizeAsync() => ResponseService.Summarize(this.Context);


        // TODO : still using this?
        [Command("list")]
        public Task ListAsync() => ReplyAsync(ResponseService.GetAllChoices());

        [Command("values")]
        public Task ValuesAsync(params string[] objects) => ReplyAsync(ResponseService.Values(objects));



    }



}
