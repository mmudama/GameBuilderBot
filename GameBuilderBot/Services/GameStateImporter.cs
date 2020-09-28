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
        private readonly GameBuilderBotConfig _botConfig;

        public GameStateImporter(IServiceProvider service)
        {
            _botConfig = service.GetRequiredService<GameBuilderBotConfig>();
            _serializer = service.GetRequiredService<Serializer>();
        }

        public void LoadGameState(GameDefinition config, ICommandContext discordContext)
        {
            string fileName = string.Format("{0}\\GameBuilderBot.{1}.{2}.json", _botConfig.GameStateDirectory,
                discordContext.Channel.Id, StringUtils.SanitizeForFileName(config.Name));

            GameState gameState = _serializer.DeserializeFromFile<GameState>(fileName, FileType.JSON);

            // TODO loop through these so that existing new variables are not removed?
            config.Fields = gameState.Fields;
        }
    }
}
