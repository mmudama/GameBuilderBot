﻿using System.Collections.Generic;
using GameBuilderBot.Common;
using GameBuilderBot.Models;

namespace GameBuilderBot.Services
{
    public class IngestionService
    {
        public static GameDefinition Ingest(string fileName, Serializer serializer)
        {
            var gameEventMap = new Dictionary<string, GameEvent>();
            var fields = new Dictionary<string, Field>();

            GameFile gameFile = serializer.DeserializeFromFile<GameFile>(fileName, FileType.YAML);

            foreach (var key in gameFile.Fields.Keys)
            {
                fields[key.ToLower()] = new Field(gameFile.Fields[key]);
            }


            foreach (GameEventIngest e in gameFile.GameEvents)
            {
                GameEvent gameEvent = new GameEvent(e);
                gameEventMap[e.Name.ToLower()] = gameEvent;
            }

            foreach (GameEvent c in gameEventMap.Values)
            {
                foreach (Outcome o in c.outcomeMap.Values)
                {
                    o.Complete(gameEventMap);
                }
            }

            return new GameDefinition(gameFile.Name, gameEventMap, fields);
        }
    }
}
