using GameBuilderBot.Services;
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
        /// Contains all available <seealso cref="Choice"/> objects for this game.
        /// </summary>
        public Dictionary<string, Choice> ChoiceMap;

        /// <summary>
        /// Constructor that populates all members
        /// </summary>
        /// <param name="name"></param>
        /// <param name="choiceMap"></param>
        public GameDefinition(string name, Dictionary<string, Choice> choiceMap)
        {
            ChoiceMap = choiceMap;
            Name = name;
        }
    }
}
