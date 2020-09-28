using Discord.Commands;
using GameBuilderBot.Common;
using GameBuilderBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GameBuilderBot.Services
{
    public class ExportService
    {
        private GameStateExporter _exporter;
        private Serializer _serializer;
        private IServiceProvider _service;

        public ExportService(IServiceProvider services)
        {
            _serializer = services.GetRequiredService<Serializer>();
            _service = services;
        }

        public string ExportGameConfigToFile(FileType fileType, GameDefinition config)
        {
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

            return _serializer.SerializeToString(game, fileType);
        }

        public void ExportGameState(GameDefinition config, ICommandContext discordContext)
        {
            new GameStateExporterJsonFile(_service).SaveGameState(config, discordContext);
        }
    }
}
