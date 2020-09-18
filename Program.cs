using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordOregonTrail.Models;
using DiscordOregonTrail.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordOregonTrail
{
    public class Program
    {

        private static string _fileName;

        private DiscordSocketClient _client;

        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                _fileName = args[0];
            }

            Console.WriteLine("Before first call");
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            using var services = ConfigureServices(_fileName);

            Console.WriteLine("After configuring services");

            _client = services.GetRequiredService<DiscordSocketClient>();

            _client.Log += LogAsync;


            //TODO use logs instead of console writes?
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await _client.LoginAsync(TokenType.Bot,
                Environment.GetEnvironmentVariable("DiscordToken")
                );
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

        private ServiceProvider ConfigureServices(string fileName)
        {
            Config config = new Config(fileName);

            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton(c => config)
                .AddSingleton(c => new ResponseService(config))
                .BuildServiceProvider();
        }
    }
}
