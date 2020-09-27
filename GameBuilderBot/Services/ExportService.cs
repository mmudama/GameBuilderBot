using GameBuilderBot.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace GameBuilderBot.Services
{
    public enum FileType
    {
        JSON,
        YAML
    }

    public class ExportService
    {
        public static string Export(FileType fileType, GameConfig config)
        {
            string output = "";
            GameFile game = new GameFile();

            var choices = new List<ChoiceIngest>();
            game.Fields = new Dictionary<string, FieldIngest>();

            foreach (var key in config.Fields.Keys)
            {
                game.Fields[key] = new FieldIngest(config.Fields[key].Expression, config.Fields[key].Value);
            }

            foreach (Choice c in config.ChoiceMap.Values)
            {
                ChoiceIngest choice = new ChoiceIngest();
                choice.Name = c.Name;
                choice.Distribution = c.Distribution;
                choice.Text = c.Text;
                choice.IsPrimary = c.IsPrimary;
                choice.Description = c.Description;

                List<OutcomeIngest> outcomes = new List<OutcomeIngest>();
                foreach (Outcome o in c.outcomeMap.Values)
                {
                    OutcomeIngest oi = new OutcomeIngest();
                    oi.Name = o.Name;
                    oi.Weight = o.Weight;
                    oi.Text = o.Text;
                    oi.Choice = o.Choice;
                    oi.Rolls = o.Rolls;
                    outcomes.Add(oi);
                }
                choice.Outcomes = outcomes.ToArray();
                choices.Add(choice);
            }

            game.Choices = choices.ToArray();

            if (fileType == FileType.JSON)
            {
                output = JsonSerializer.Serialize(game);
            }
            else if (fileType == FileType.YAML)
            {
                var serializer = new YamlDotNet.Serialization.Serializer();
                output = serializer.Serialize(game);
            }
            else output = "Invalid file type";

            return output;
        }
    }
}
