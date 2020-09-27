using GameBuilderBot.Common;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GameBuilderBot.Services
{
    class GameStateExporterJsonFile : GameStateExporter
    {
        public override void SaveGameStateConcrete(GameState gameState)
        {
            string fileName = string.Format("c:\\temp\\GameBuilderBot.{0}.json", DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss"));

            try
            {

                fileName = string.Format("c:\\Temp\\GameBuilderBot.{0}.{1}.json", gameState.ChannelId, StringUtils.SanitizeForFileName(gameState.Name));
                string stateAsString = JsonConvert.SerializeObject(gameState);
                var streamWriter = new StreamWriter(fileName);
                streamWriter.Write(stateAsString);
                streamWriter.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Could not save GameState to file : {0}", fileName));
                Console.WriteLine(e.Message);

                throw new GameBuilderBotException("Could not save game state; your data will not be available for reload");
            }
        }
    }
}
