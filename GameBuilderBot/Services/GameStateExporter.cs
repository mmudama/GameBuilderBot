using Discord.Commands;
using Discord.WebSocket;
using GameBuilderBot.Exceptions;
using GameBuilderBot.Models;

namespace GameBuilderBot.Services
{
    /// <summary>
    /// In theory, this should be extended for various data stores (file, database, etc)
    /// </summary>
    public abstract class GameStateExporter
    {
        public void SaveGameState(GameState gameState, ICommandContext discordContext)
        {
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
