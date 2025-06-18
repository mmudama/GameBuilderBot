using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameBuilderBot.Common;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using Microsoft.Extensions.DependencyInjection;

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

        public bool LoadGameState(ulong channelId)
        {
            GameDefinition definition = GetGameDefinitionForChannelId(channelId);
            GameState gameState = new();
            bool fileFound = false;

            // TODO This filename format is defined both here and in GameStateExporterJsonFile. Put it in some common place.
            string fileName = string.Format("{0}\\GameBuilderBot.{1}.{2}.json", Config.GameStateDirectory,
    channelId, StringUtils.SanitizeForFileName(definition.Name));

            try
            {
                // Load the GameState if there's a save file for this game in this Discord channel
                gameState = _serializer.DeserializeFromFile<GameState>(fileName, FileType.JSON);
                fileFound = true;
            }
            catch (FileNotFoundException)
            {
                // Create a GameState if we didn't find a file
                gameState = new GameState
                {
                    Name = definition.Name,
                    Fields = new Dictionary<string, Field>(),
                    ChannelId = channelId,

                    // FriendlyName will be overwritten with the correct value when the game saves a variable
                    // Could pull it from SocketCommandContext, but is it worth adding another parameter up the call stack?
                    FriendlyName = "default"
                };

                ApplyDefaultValuesToGameState(definition.Fields, gameState.Fields);

                _gameStateMap[new StateIdentifier(channelId, definition)] = gameState;

            }
            return fileFound;
        }

        /// <summary>
        /// Copies Field values from the source to the destination
        /// For each key/value pair in the source that does not exist in the destination, attempts to copy
        /// the field and add it to the destination.
        /// </summary>
        /// <param name="source">The source dictionary containing default fields.</param>
        /// <param name="destination">The destination dictionary to populate with missing fields.</param>
        private void ApplyDefaultValuesToGameState(Dictionary<string, Field> source, Dictionary<string, Field> destination)
        {
            foreach (var fieldName in source.Keys)
            {
                if (!destination.ContainsKey(fieldName))
                {
                    Field f = default;
                    try
                    {
                        f = _serializer.TryDeepCopy(source[fieldName]);
                        destination[fieldName] = f;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Could not add default variable {fieldName} to current game");
                    }
                    destination[fieldName] = f;
                }
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
