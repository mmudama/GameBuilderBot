using System.Collections.Generic;

namespace DiscordOregonTrail.Models
{
    public class Config
    {
        protected Choice[] Choices { get; set; }
        public Dictionary<string, Choice> choiceMap = new Dictionary<string, Choice>();

        public Config(string fileName)
        {
            var fileContents = System.IO.File.ReadAllText(fileName);

            var deserializer = new YamlDotNet.Serialization.Deserializer();
            Choices = deserializer.Deserialize<Choice[]>(fileContents);

            foreach (Choice c in Choices)
            {
                c.Complete();
                choiceMap[c.Name.ToLower()] = c;
            }

            foreach (Choice c in Choices)
            {
                foreach (Outcome o in c.Outcomes)
                {
                    if (o.Choice != null)
                    {
                        o.ChildChoice = choiceMap[o.Choice.ToLower()];
                    }
                }
            }
        }

    }
}
