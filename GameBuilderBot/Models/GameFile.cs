using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class GameFile
    {
        public ChoiceIngest[] Choices;
        public Dictionary<string, FieldIngest> Fields;

        public GameFile() { }

    }
}
