using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// GameDefinition provides the structure of a game. It determines the parameters available
    /// via `!game`, what the possible outcomes of that call will be, and their probabilities.
    /// See the Example directory for ideas.
    /// </summary>
    /// <seealso cref="GameFile"/>
    public class GameDefinition
    {
        /// <summary>
        /// "Friendly" name to identify the game, like "The Quest for the Holy Grail" or
        /// "Robot Hijinx." This will be exposed directly in chat.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contains all available <seealso cref="GameEvent"/> objects for this game.
        /// </summary>
        public Dictionary<string, GameEvent> GameEventMap;

        /// <summary>
        /// Constructor that populates all members
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gameEventMap"></param>
        public GameDefinition(string name, Dictionary<string, GameEvent> gameEventMap)
        {
            GameEventMap = gameEventMap;
            Name = name;
        }
    }
}
