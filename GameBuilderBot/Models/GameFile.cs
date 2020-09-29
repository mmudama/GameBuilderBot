using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class GameFile
    {
        public string Name { get; set; }
        public ChoiceIngest[] Choices { get; set; }
        public Dictionary<string, FieldIngest> Fields { get; set; }

        public GameFile() { }
    }
}
