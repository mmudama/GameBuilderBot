using GameBuilderBot.Common;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GameBuilderBot.Services
{
    internal class GameStateExporterJsonFile : GameStateExporter
    {
        protected Serializer _serializer;
        protected GameBuilderBotConfig _gbbConfig;

        public GameStateExporterJsonFile(IServiceProvider service)
        {
            _serializer = service.GetRequiredService<Serializer>();
            _gbbConfig = service.GetRequiredService<GameBuilderBotConfig>();

        }

        public override void SaveGameStateConcrete(GameState gameState)
        {
            string fileName = string.Format("{0}\\GameBuilderBot.{1}.json", _gbbConfig.GameStateDirectory, 
                DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss"));

            try
            {
                fileName = string.Format("{0}\\GameBuilderBot.{1}.{2}.json", _gbbConfig.GameStateDirectory, 
                    gameState.ChannelId, StringUtils.SanitizeForFileName(gameState.Name));

                _serializer.SerializeToFile(gameState, FileType.JSON, fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Could not save GameState to file : {0}", fileName));
                Console.WriteLine(e.Message);

                throw new GameBuilderBotException("Could not save game state; your data will not be available for reload");
            }
        }
    }
}
