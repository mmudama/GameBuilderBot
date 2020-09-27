using Discord.Commands;
using Discord.WebSocket;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static void SaveGameState(GameConfig config, SocketCommandContext discordContext)
        {
            var gameState = new GameState();
            gameState.Fields = config.Fields;
            gameState.ChannelId = discordContext.Channel.Id;
            gameState.Name = config.Name;

            if (discordContext.Channel is SocketTextChannel)
            {
                gameState.FriendlyName = GetFriendlyName((SocketTextChannel)discordContext.Channel);
            }
            else if (discordContext.Channel is SocketDMChannel)
            {
                gameState.FriendlyName = GetFriendlyName((SocketDMChannel)discordContext.Channel);
            }
            else throw new GameBuilderBotException("Unrecognized channel type; cannot save state");

            SaveGameStateToFile(gameState);
        }

        private static string GetFriendlyName(SocketTextChannel channel)
        {
            return string.Format("{0};{1}", channel.Guild.Name, channel.Name);
        }

        private static string GetFriendlyName(SocketDMChannel channel)
        {
            return channel.Recipient.Username;
        }

        protected static void SaveGameStateToFile(GameState state) {


            string fileName = string.Format("c:\\temp\\GameBuilderBot.{0}.json", DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss"));

            try
            {
                fileName = string.Format("c:\\Temp\\GameBuilderBot.{0}.json", state.ChannelId);
                string stateAsString = JsonSerializer.Serialize(state);
                var streamWriter = new StreamWriter(fileName);
                streamWriter.Write(stateAsString);
                streamWriter.Close();
            } catch (Exception e)
            {
                Console.WriteLine(String.Format("Could not save GameState to file : {0}", fileName));
                Console.WriteLine(e.Message);

                throw new GameBuilderBotException("Could not save game state; your data will not be available for reload");
            }
        }
    }
}
