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
        
        // TODO remove when multiple games are supported
        [Obsolete("Will be removed when GBB supports multiple loaded game definitions")]
        public string GameDefinitionFile { get; set; }

        private Dictionary<string, GameDefinition> gameDefinitions = new Dictionary<string, GameDefinition>();

        // TODO this key is not sufficient to identify gameStates
        // maybe a map of GameDefinition names to channel ids to gameStates
        private Dictionary<string, GameState> gameStates = new Dictionary<string, GameState>();

        public void Init(Serializer serializer)
        {
            var definitionFiles = Directory.GetFiles(GameDefinitionDirectory);

            foreach (string file in definitionFiles)
            {
                GameFile throwaway = serializer.DeserializeFromFile<GameFile>(file, FileType.YAML);
            }
        }
    }
}
