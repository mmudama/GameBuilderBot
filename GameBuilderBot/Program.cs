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
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly CommandHandlingService _commandHandlingService;
        private readonly GameHandlingService _gameHandlingService;

        public static void Main(string[] _)
        {
            var services = ConfigureServices();
            var program = services.GetRequiredService<Program>();
            program.MainAsync().GetAwaiter().GetResult();
        }

        public Program(
            DiscordSocketClient client, 
            CommandService commandService, 
            CommandHandlingService commandHandlingService, 
            GameHandlingService gameHandlingService)
        {
            _client = client;
            _commandService = commandService;
            _commandHandlingService = commandHandlingService;
            _gameHandlingService = gameHandlingService;
        }

        public async Task MainAsync()
        {
            using var services = ConfigureServices();
            
            _client.Log += LogAsync;

            //TODO use logs instead of console writes
            _commandService.Log += LogAsync;
            
            await _client.LoginAsync(TokenType.Bot, _gameHandlingService.Config.DiscordBotToken);
            await _client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await _commandHandlingService.InitializeAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketConfig { 
                    GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.GuildMessages 
                })
                .AddSingleton<Program>()
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
