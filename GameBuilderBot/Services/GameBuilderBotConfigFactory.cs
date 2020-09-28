using GameBuilderBot.Common;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using System;

namespace GameBuilderBot.Services
{
    public class GameBuilderBotConfigFactory
    {
        private Serializer _serializer;

        public GameBuilderBotConfigFactory(Serializer serializer)
        {
            _serializer = serializer;
        }

        public GameBuilderBotConfig GetBotConfig(string fileName)
        {
            try
            {
                GameBuilderBotConfig botConfig = _serializer.DeserializeFromFile<GameBuilderBotConfig>(fileName, FileType.YAML);
                botConfig.Init(_serializer);
                return botConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new GameBuilderBotException("Could not initialize app; " + e.Message);
            }
        }
    }
}
