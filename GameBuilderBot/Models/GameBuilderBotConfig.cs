using GameBuilderBot.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameBuilderBot.Models
{
    public class GameBuilderBotConfig
    {
        public string DiscordBotToken { get; set; }
        public string GameDefinitionDirectory { get; set; }
        public string GameStateDirectory { get; set; }

        private Dictionary<string, GameConfig> gameDefinitions = new Dictionary<string, GameConfig>();

        // TODO this probably needs a smarter lookup, but maybe just join string for key?
        private Dictionary<string, GameState> gameStates = new Dictionary<string, GameState>();

        public void Init(Serializer serializer)
        {
            var definitionFiles = Directory.GetFiles(GameDefinitionDirectory);

            foreach (string file in definitionFiles)
            {
                GameFile throwaway = serializer.DeserializeFromFile<GameFile>(file, FileType.YAML);
                Console.WriteLine("BREAK");
            }
        }
    }
}
