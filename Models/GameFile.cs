using System.Collections.Generic;

namespace DiscordOregonTrail.Models
{
    public class GameFile
    {
        public Choice[] Choices { get; private set; }
        public Dictionary<string, Fields> Fields;

        public GameFile() { }

    }
}
