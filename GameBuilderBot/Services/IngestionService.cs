using GameBuilderBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameBuilderBot.Services
{
    public class IngestionService
    {

        public static Config Ingest(string fileName)
        {
            var choiceMap = new Dictionary<string, Choice>();
            var fields = new Dictionary<string, Field>();

            var fileContents = File.ReadAllText(fileName);

            var deserializer = new YamlDotNet.Serialization.Deserializer();
            GameFile gameFile = deserializer.Deserialize<GameFile>(fileContents);

            foreach (var key in gameFile.Fields.Keys)
            {
                fields[key] = new Field(gameFile.Fields[key]);
            }

            foreach (ChoiceIngest c in gameFile.Choices)
            {
                Choice choice = new Choice(c);
                choice.Complete();
                choiceMap[c.Name.ToLower()] = choice;
            }

            foreach (Choice c in choiceMap.Values)
            {
                foreach (Outcome o in c.outcomeMap.Values)
                {
                    o.Complete(choiceMap);
                }
            }

            foreach (Choice c in choiceMap.Values)
            {
                Console.WriteLine(c.GetSummary());
            }

            return new Config(choiceMap, fields);

        }

    }
}
