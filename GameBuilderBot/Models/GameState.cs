using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class GameState
    {
        public string Name { get; set; }
        public Dictionary<string, Field> Fields { get; set; }
        public ulong ChannelId { get; set; }
        public string FriendlyName { get; set; }
    }
}
