using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DiscordOregonTrail.Models
{
    public class Config
    {
        protected Choice[] choices { get; set; }
        public Dictionary<string, Choice> choiceMap = new Dictionary<string, Choice>();

        public Config(string fileName)
        {
            var fileContents = System.IO.File.ReadAllText(fileName);

            var deserializer = new YamlDotNet.Serialization.Deserializer();
            choices = deserializer.Deserialize<Choice[]>(fileContents);

            foreach (Choice c in choices)
            {
                c.Complete();
                choiceMap[c.Name.ToLower()] = c;
            }

            foreach (Choice c in choices)
            {
                foreach (Outcome o in c.Outcomes)
                {
                    if (o.Choice != null)
                    {
                        o._choice = choiceMap[o.Choice.ToLower()];
                    }
                }
            }

            Console.WriteLine("BREAK");

        }

    }
}
