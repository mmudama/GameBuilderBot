using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameBuilderBot.Common;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GameBuilderBot.Services
{
    public struct StateIdentifier(ulong channelId, GameDefinition gameDefinition)
    {
        private readonly ulong ChannelId = channelId;
        private readonly GameDefinition GameDefinition = gameDefinition;

        public override bool Equals(object obj) =>
        obj is StateIdentifier stateIdentifier
         && stateIdentifier.ChannelId == ChannelId
         && stateIdentifier.GameDefinition.Equals(GameDefinition);

        public override int GetHashCode()
        {
            return HashCode.Combine(ChannelId, GameDefinition);
        }
    }

    public class GameHandlingService
    {
        private const string ENV_GBB_CONFIG_FILE = "GBB_CONFIG_FILE";

        private readonly Serializer _serializer;

        // _gameDefinitionMap[gameName]
        private readonly Dictionary<string, GameDefinition> _gameDefinitionMap;

        private readonly List<GameDefinition> _gameDefinitionList;

        // _activeGames[channelId]
        private readonly Dictionary<ulong, GameDefinition> _activeGames = new();

        //_gameStateMap[new StateIdentifier(channelId, gameDefinition)]
        private readonly Dictionary<StateIdentifier, GameState> _gameStateMap = new();

        public GameBuilderBotConfig Config;

        public GameHandlingService(IServiceProvider services)
        {
            _serializer = services.GetRequiredService<Serializer>();

            string appConfigFileName = Environment.GetEnvironmentVariable(ENV_GBB_CONFIG_FILE);

            Config = _serializer.DeserializeFromFile<GameBuilderBotConfig>(appConfigFileName, FileType.YAML);
            _gameDefinitionMap = PopulateGameDefinitions(Config);
            _gameDefinitionList = _gameDefinitionMap.Values.ToList();
        }

        // TODO this isn't the right place and should the Serializer class maybe be static anyway?
        public Serializer GetSerializer()
        {
            return _serializer;

        }


        public void LoadGameState(ulong channelId, out bool fileFound)
        {
            GameDefinition definition = GetGameDefinitionForChannelId(channelId);

            // TODO This filename format is defined both here and in GameStateExporterJsonFile. Put it in some common place.
            string fileName = string.Format("{0}\\GameBuilderBot.{1}.{2}.json", Config.GameStateDirectory,
    channelId, StringUtils.SanitizeForFileName(definition.Name));

            GameState gameState = new GameState();

            try
            {
                gameState = _serializer.DeserializeFromFile<GameState>(fileName, FileType.JSON);

                fileFound = true;
            }
            catch (FileNotFoundException)
            {
                fileFound = false;

                if (!fileFound)
                {
                    gameState = new GameState
                    {
                        Name = definition.Name,
                        Fields = new Dictionary<string, Field>(),
                        ChannelId = channelId,
                        FriendlyName = "default"
                    };
                }

                foreach (var fieldName in definition.Fields.Keys)
                {
                    if (!gameState.Fields.ContainsKey(fieldName))
                    {
                        string s = _serializer.SerializeToString(definition.Fields[fieldName], FileType.JSON);
                        Field f = _serializer.DeserializeFromString<Field>(s, FileType.JSON);
                        gameState.Fields[fieldName] = f;
                        //gameState.Fields[fieldName] = definition.Fields[fieldName];

                    }
                }

                _gameStateMap[new StateIdentifier(channelId, definition)] = gameState;

            }
        }

        public GameDefinition GetGameDefinitionForChannelId(ulong channelId)
        {
            if (_activeGames.ContainsKey(channelId))
            {
                return _activeGames[channelId];
            }
            else
            {
                throw new NoActiveGameException(message: string.Format("Could not find active game for this channel", channelId));
            }
        }

        public void SetGameDefinitionForChannelId(ulong channelId, int index)
        {
            if (index > -1 && index < _gameDefinitionList.Count)
            {
                _activeGames[channelId] = _gameDefinitionList[index];
            }
            else
            {
                throw new IndexOutOfRangeException(String.Format("Game {0} is not a valid choice", index));
            }
        }


        public string RemoveGameDefinitionForChannelId(ulong channelId)
        {

            string name = GetGameDefinitionForChannelId(channelId).Name;
            _activeGames.Remove(channelId);
            return name;
        }



        public List<GameDefinition> GetAllGameDefinitions()
        {
            return _gameDefinitionList;
        }

        public GameState GetGameStateForActiveGame(ulong channelId)
        {
            GameState response = null;

            if (_activeGames.TryGetValue(channelId, out GameDefinition game))
            {
                if (_gameStateMap.TryGetValue(new StateIdentifier(channelId, game), out GameState state))
                {
                    return state;
                }

            }
            else
            {
                throw new NoActiveGameException();
            }
            return response;
        }

        private Dictionary<string, GameDefinition> PopulateGameDefinitions(GameBuilderBotConfig botConfig)
        {
            var gameDefinitionMap = new Dictionary<string, GameDefinition>();

            var definitionFiles = Directory.GetFiles(botConfig.GameDefinitionDirectory);

            foreach (string fileName in definitionFiles)
            {
                GameDefinition current = IngestionService.Ingest(fileName, _serializer);
                if (gameDefinitionMap.ContainsKey(current.Name))
                {
                    Console.WriteLine("Duplicate game definition {0}; ignoring {1}", current.Name, fileName);
                }
                else
                {
                    gameDefinitionMap[current.Name] = current;
                }

            }

            return gameDefinitionMap;
        }
    }
}
