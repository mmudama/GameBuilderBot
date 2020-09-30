using GameBuilderBot.Common;
using GameBuilderBot.Models;
using System;
using System.Collections.Generic;

namespace GameBuilderBot.Services
{
    public class IngestionService
    {
        public static (GameDefinition, GameState) Ingest(string fileName, Serializer serializer)
        {
            var choiceMap = new Dictionary<string, Choice>();
            var fields = new Dictionary<string, Field>();

            GameFile gameFile = serializer.DeserializeFromFile<GameFile>(fileName, FileType.YAML);

            foreach (var key in gameFile.Fields.Keys)
            {
                fields[key.ToLower()] = new Field(gameFile.Fields[key]);
            }

            GameState defaultState = new GameState();
            defaultState.Name = gameFile.Name;
            defaultState.ChannelId = 0;
            defaultState.Fields = fields;

            foreach (ChoiceIngest c in gameFile.Choices)
            {
                Choice choice = new Choice(c);
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
                // TODO do this differently - currently GetSummary can cause a stack overflow exception with nested choices
                //Console.WriteLine(c.GetSummary());
            }

            return (new GameDefinition(gameFile.Name, choiceMap), defaultState);
        }
    }
}
