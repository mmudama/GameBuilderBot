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

			new Program().MainAsync().GetAwaiter().GetResult();
		}

        private static void LoadConfiguration(string fileName)
        {

        }
        

        public async Task MainAsync()
		{
			using (var services = ConfigureServices(_fileName))
			{
				_client = services.GetRequiredService<DiscordSocketClient>();

				_client.Log += LogAsync;

				// MONIQUE: what does this actually do?
				services.GetRequiredService<CommandService>().Log += LogAsync;



				// Remember to keep token private or to read it from an 
				// external source! In this case, we are reading the token 
				// from an environment variable. If you do not know how to set-up
				// environment variables, you may find more information on the 
				// Internet or by using other methods such as reading from 
				// a configuration.
				await _client.LoginAsync(TokenType.Bot,
					Environment.GetEnvironmentVariable("DiscordToken")
					);
				await _client.StartAsync();

				// Here we initialize the logic required to register our commands.
				await services.GetRequiredService<CommandHandlingService>().InitializeAsync();


				// Block this task until the program is closed.
				await Task.Delay(-1);
			}
		}

		private Task LogAsync(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private ServiceProvider ConfigureServices(string fileName)
		{
            return new ServiceCollection()
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<CommandService>()
				.AddSingleton<CommandHandlingService>()
				.AddSingleton<HttpClient>()
				.AddSingleton<Config>(c => new Config(fileName))
				.BuildServiceProvider();
		}
	}
}
