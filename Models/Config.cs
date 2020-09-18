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
            var json = System.IO.File.ReadAllText(fileName);
            choices = JsonSerializer.Deserialize<Choice[]>(json);

            Console.Out.WriteLine("Break Here");

            foreach (Choice c in choices)
            {
                c.Complete();
                choiceMap[c.Name.ToLower()] = c;
            }

        }

    }
}
