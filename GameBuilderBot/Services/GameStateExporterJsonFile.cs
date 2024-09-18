using System;
using GameBuilderBot.Common;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GameBuilderBot.Services
{
    internal class GameStateExporterJsonFile : GameStateExporter
    {
        protected Serializer _serializer;
        protected GameHandlingService _gameHandler;

        public GameStateExporterJsonFile(IServiceProvider service)
        {
            _serializer = service.GetRequiredService<Serializer>();
            _gameHandler = service.GetRequiredService<GameHandlingService>();
        }

        public override void SaveGameStateConcrete(GameState gameState)
        {
            string fileName = string.Format("{0}\\GameBuilderBot.{1}.json", _gameHandler.Config.GameStateDirectory,
                DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss"));

            try
            {
                fileName = string.Format("{0}\\GameBuilderBot.{1}.{2}.json", _gameHandler.Config.GameStateDirectory,
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
