using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GameBuilderBot.Common;
using GameBuilderBot.Models;
using GameBuilderBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GameBuilderBot
{
    public class Program
    {
        private static string _fileName;
        private static GameBuilderBotConfig _botConfig;

        private const string ENV_BOT_CONFIG_FILE = "GBB_CONFIG_FILE";

        private DiscordSocketClient _client;

        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                _fileName = args[0];
            }

            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            using var services = ConfigureServices();

            _client = services.GetRequiredService<DiscordSocketClient>();
            _client.Log += LogAsync;

            //TODO use logs instead of console writes?
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await _client.LoginAsync(TokenType.Bot,  _botConfig.DiscordBotToken);
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
            Serializer serializer = new Serializer();

            string appConfigFileName = Environment.GetEnvironmentVariable(ENV_BOT_CONFIG_FILE);
            _botConfig = new GameBuilderBotConfigFactory(serializer).GetBotConfig(appConfigFileName);

            GameDefinition gameDefinition = IngestionService.Ingest(_botConfig.GameDefinitionDirectory
                + _botConfig.GameDefinitionFile, serializer);

            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton(gd => gameDefinition)
                .AddSingleton<ResponseService>()
                .AddSingleton<ExportService>()
                .AddSingleton<GameStateImporter>()
                .AddSingleton<Serializer>()
                .AddSingleton(bc => _botConfig)
                .BuildServiceProvider();
        }
    }
}
