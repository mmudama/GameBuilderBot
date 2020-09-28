using Discord.Commands;
using Discord.WebSocket;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;

namespace GameBuilderBot.Services
{
    public abstract class GameStateExporter
    {
        public void SaveGameState(GameConfig config, ICommandContext discordContext)
        {
            var gameState = new GameState();
            gameState.ChannelId = discordContext.Channel.Id;
            gameState.Name = config.Name;
            gameState.Fields = config.Fields;

            if (discordContext.Channel is SocketTextChannel)
            {
                gameState.FriendlyName = GetFriendlyName((SocketTextChannel)discordContext.Channel);
            }
            else if (discordContext.Channel is SocketDMChannel)
            {
                gameState.FriendlyName = GetFriendlyName((SocketDMChannel)discordContext.Channel);
            }
            else throw new GameBuilderBotException("Unrecognized channel type; cannot save state");

            SaveGameStateConcrete(gameState);
        }

        public abstract void SaveGameStateConcrete(GameState gameState);

        protected static string GetFriendlyName(SocketTextChannel channel)
        {
            return string.Format("{0};{1}", channel.Guild.Name, channel.Name);
        }

        protected static string GetFriendlyName(SocketDMChannel channel)
        {
            return channel.Recipient.Username;
        }
    }
}
