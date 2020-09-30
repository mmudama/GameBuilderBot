using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class GameBuilderBotConfig
    {
        public string DiscordBotToken { get; set; }
        public string GameDefinitionDirectory { get; set; }
        public string GameStateDirectory { get; set; }

        // TODO override by channel, regardless of active game 
        public List<char> ValidCommandPrefixes { get; set; }
    }
}
