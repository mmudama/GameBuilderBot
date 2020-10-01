using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// Represents the configuration file loaded at app launch.
    /// </summary>
    public class GameBuilderBotConfig
    {
        /// <summary>
        /// Each Discord bot has its own token. Multiple apps can be running with the same token,
        /// in which case all of them respond. At startup, the token's bot will come online
        /// and be available through chat.
        /// </summary>
        public string DiscordBotToken { get; set; }

        /// <summary>
        /// Directory from which the app will load all <seealso cref="GameDefinition"/> objects at startup.
        /// </summary>
        public string GameDefinitionDirectory { get; set; }

        /// <summary>
        /// Directory from which the app will load all <seealso cref="GameState"/> objects on demand, and
        /// to which it will save these objects whenever a game's state is updated.
        /// </summary>
        public string GameStateDirectory { get; set; }

        /// <summary>
        /// Not yet implemented. This should allow a channel to specify the character prefix for all their games
        /// </summary>
        // TODO override by channel, regardless of active game 
        public List<char> ValidCommandPrefixes { get; set; }
    }
}
