using Discord.Commands;
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

        GameStateExporter exporter;

        public ExportService()
        {

        }

        public string ExportGameConfigToFile(FileType fileType, GameConfig config)
        {
            string output;
            GameFile game = new GameFile();

            var choices = new List<ChoiceIngest>();
            game.Fields = new Dictionary<string, FieldIngest>();

            foreach (var key in config.Fields.Keys)
            {
                game.Fields[key] = new FieldIngest(config.Fields[key].Expression, config.Fields[key].Value);
            }

            foreach (Choice c in config.ChoiceMap.Values)
            {
                ChoiceIngest choice = new ChoiceIngest
                {
                    Name = c.Name,
                    Distribution = c.Distribution,
                    Text = c.Text,
                    IsPrimary = c.IsPrimary,
                    Description = c.Description
                };

                List<OutcomeIngest> outcomes = new List<OutcomeIngest>();
                foreach (Outcome o in c.outcomeMap.Values)
                {
                    outcomes.Add(
                        new OutcomeIngest
                        {
                            Name = o.Name,
                            Weight = o.Weight,
                            Text = o.Text,
                            Choice = o.Choice,
                            Rolls = o.Rolls
                        });
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

        public void ExportGameState(GameConfig config, ICommandContext discordContext)
        {
            new GameStateExporterJsonFile().SaveGameState(config, discordContext);

        }
    }
}
