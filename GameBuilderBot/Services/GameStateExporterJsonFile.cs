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

        public GameStateExporterJsonFile(IServiceProvider service)
        {
            _serializer = service.GetRequiredService<Serializer>();
        }

        public override void SaveGameStateConcrete(GameState gameState)
        {
            string fileName = string.Format("c:\\temp\\GameBuilderBot.{0}.json", DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss"));

            try
            {
                fileName = string.Format("c:\\Temp\\GameBuilderBot.{0}.{1}.json", gameState.ChannelId, StringUtils.SanitizeForFileName(gameState.Name));

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
