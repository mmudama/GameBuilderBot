using Discord.Commands;
using GameBuilderBot.Common;
using GameBuilderBot.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GameBuilderBot.Services
{
    class GameStateImporter
    {
        public GameStateImporter()
        {

        }

        public void LoadGameState(GameConfig config, ICommandContext discordContext)
        {
            string fileName = string.Format("c:\\Temp\\GameBuilderBot.{0}.{1}.json", discordContext.Channel.Id, StringUtils.SanitizeForFileName(config.Name));

            string json = File.ReadAllText(fileName);

            GameState gameState = JsonConvert.DeserializeObject<GameState>(json);

            config.Fields = gameState.Fields;

            Console.WriteLine("BREAK");

        }
    }
}
