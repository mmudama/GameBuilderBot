using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class GameFile
    {
        public ChoiceIngest[] Choices { get; private set; }
        public Dictionary<string, FieldIngest> Fields;

        public GameFile() { }

    }
}
