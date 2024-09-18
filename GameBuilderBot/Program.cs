using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GameBuilderBot.Common;
using GameBuilderBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GameBuilderBot
{
    public class Program
    {
        private DiscordSocketClient _client;

        public static void Main(string[] _)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            using var services = ConfigureServices();

            _client = services.GetRequiredService<DiscordSocketClient>();
            _client.Log += LogAsync;

            //TODO use logs instead of console writes
            services.GetRequiredService<CommandService>().Log += LogAsync;
            GameHandlingService gameHandler = services.GetRequiredService<GameHandlingService>();

            await _client.LoginAsync(TokenType.Bot, gameHandler.Config.DiscordBotToken);
            await _client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<ResponseService>()
                .AddSingleton<ExportService>()
                .AddSingleton<Serializer>()
                .AddSingleton<GameHandlingService>()
                .AddSingleton<IngestionService>()
                .BuildServiceProvider();
        }
    }
}
