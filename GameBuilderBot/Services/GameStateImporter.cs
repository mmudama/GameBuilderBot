using Discord.Commands;
using GameBuilderBot.Common;
using GameBuilderBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GameBuilderBot.Services
{
    internal class GameStateImporter
    {
        private Serializer _serializer;

        public GameStateImporter(IServiceProvider service)
        {
            _serializer = service.GetRequiredService<Serializer>();
        }

        public void LoadGameState(GameConfig config, ICommandContext discordContext)
        {
            string fileName = string.Format("c:\\Temp\\GameBuilderBot.{0}.{1}.json", discordContext.Channel.Id, StringUtils.SanitizeForFileName(config.Name));

            GameState gameState = _serializer.DeserializeFromFile<GameState>(fileName, FileType.JSON);

            config.Fields = gameState.Fields;

            Console.WriteLine("BREAK");
        }
    }
}
