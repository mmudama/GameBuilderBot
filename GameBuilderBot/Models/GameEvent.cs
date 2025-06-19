using System.Collections.Generic;
using System.Text;
using GameBuilderBot.Services;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// A GameEvent object represents a set of possibilities ("Outcomes") available with a virtual "dice roll."
    /// GameEvents can represent chained dice rolls when one or more of the associated Outcomes results in another GameEvent.
    /// </summary>
    /// <seealso cref="Outcome"/>
    public class GameEvent
    {
        /// <summary>
        /// Any GameEvent may be
        /// invoked by a user using `!game [Name]` or, for multi word names, `!game "[Name"]`.
        /// User input is treated as case insensitive.
        /// Each GameEvent Name should map to only one GameEvent member of a <seealso cref="GameDefinition"/>
        /// or <seealso cref="GameFile"/>
        /// </summary>
        public string Name;

        /// <summary>
        /// Currently "Weighted" or "All"
        /// If the Distribution is "Weighted", the sum of all member Outcomes will be used as the die to roll.
        /// For example, if Outcome A has a weight of 5, B has 12, and C has 3, this GameEvent would use a virtual
        /// twenty-sided die to determine the outcome. B would be the most likely outcome with a 60% chance of success.
        /// 
        /// If the Distribution is "All", invoking this GameEvent will result in all Outcomes, not just one..
        /// </summary>
        public string Distribution; // TODO make this an enum

        /// <summary>
        /// Indicates whether this GameEvent should be considered a top-level GameEvent. A user can "roll" any GameEvent,
        /// but those flagged IsPrimary are considered the most relevant entry points for events. Other
        /// GameEvents would typically be present only as chained rolls that result from Outcomes.
        /// </summary>
        public bool IsPrimary;

        /// <summary>
        /// One-line description of the GameEvent, used in help messages etc
        /// </summary>
        public string Description;

        /// <summary>
        /// All Outcomes that may result from this GameEvent
        /// </summary>
        public readonly Dictionary<string, Outcome> outcomeMap = new Dictionary<string, Outcome>();

        /// <summary>
        /// Each element represents one unit of possibility. For example, if outcome A has a weight of
        /// 20, there will be 20 array elements representing outcome A. This is a cheater way to roll
        /// probabilities - a random number can be used as an index into the array.
        /// </summary>
        public string[] PossibleOutcomes;

        /// <summary>
        /// Creates an in-memory representation of a rollable event.
        /// </summary>
        /// <param name="c"><seealso cref="GameEventIngest"/>is the deserialized GameEvent field from a <seealso cref="GameFile"/></param>
        /// 
        public GameEvent(GameEventIngest c)
        {
            Name = c.Name.ToLower();
            Distribution = c.Distribution;
            IsPrimary = c.IsPrimary;
            Description = c.Description;

            foreach (OutcomeIngest o in c.Outcomes)
            {
                outcomeMap[o.Name] = new Outcome(o);
            }

            CreateProbabilityArray();
        }

        /// <summary>
        /// Uses <seealso cref="outcomeMap"/> to create the member <seealso cref="PossibleOutcomes"/>
        /// </summary>
        private void CreateProbabilityArray()
        {
            List<string> possibleOutcomes = new List<string>();

            foreach (Outcome o in outcomeMap.Values)
            {
                int count = o.Weight;
                for (int i = 0; i < count; i++)
                {
                    possibleOutcomes.Add(o.Name);
                }

                if (o.Text == null) o.Text = o.Name;
            }

            PossibleOutcomes = possibleOutcomes.ToArray();
        }

        /// <summary>
        /// Constructs human-readable output representing the outcome of a virtual dice roll.
        /// </summary>
        /// <param name="definition">The rules for the current game</param>
        /// <param name="state">Current values for the game</param>
        /// <param name="depth">Used to detect excessive nesting / possible infinite cycle</param>
        /// <returns></returns>
        private StringBuilder GetResponseForWeightedGameEvent(GameDefinition definition, GameState state, int depth)
        {
            StringBuilder response = new StringBuilder();

            int max = PossibleOutcomes.Length;
            int roll = DiceRollService.Roll(max) - 1;

            Outcome o = outcomeMap[PossibleOutcomes[roll]];

            string outcome = o.GetResponse(state);

            if (max <= 1)
            {
                response.AppendLine(outcome);
            }
            else if (max == 2)
            {
                response.AppendLine(string.Format("Flipped a coin and got: {0}", outcome));
            }
            else
            {
                response.AppendLine(string.Format("[1d{0}: {1}] {2}", max, roll + 1, outcome));
            }

            if (o.ChildGameEvent != null)
            {
                response.Append(o.ChildGameEvent.GetResponseForEventRoll(definition, state, depth));
            }

            return response;
        }

        /// <summary>
        /// Constructs human-readable output when Distribution = All (ie, all Outcomes are evaluated).
        /// </summary>
        /// <param name="definition">The rules for the current game</param>
        /// <param name="state">Current values for the game</param>
        /// <param name="depth">Used to detect excessive nesting / possible infinite cycle</param>
        /// <returns></returns>
        private StringBuilder GetResponseForDistributionAllGameEvent(GameDefinition definition, GameState state, int depth)
        {
            StringBuilder response = new StringBuilder();

            foreach (Outcome o in outcomeMap.Values)
            {
                if (o.ChildGameEvent == null)
                {
                    response.AppendLine($"{o.GetResponse(state)}");
                }
                else
                {
                    response.Append(o.ChildGameEvent.GetResponseForEventRoll(definition, state, depth));
                }
            }

            return response;
        }

        /// <summary>
        /// Constructs human-readable output representing the outcome of a virtual dice roll.
        /// </summary>
        /// <param name="definition">The rules for the current game</param>
        /// <param name="state">Current values for the game</param>
        /// <param name="depth">Used to detect excessive nesting / possible infinite cycle</param>
        /// <returns></returns>
        public string GetResponseForEventRoll(GameDefinition definition, GameState state, int depth)
        {
            depth++;

            if (depth > 20)
            {
                return "Too much nesting detected. Check your config file. Aborting!";
            }

            StringBuilder response = new StringBuilder();

            switch (Distribution)
            {
                case "Weighted":
                    response.Append($"**{Name}**: {GetResponseForWeightedGameEvent(definition, state, depth)}");
                    break;

                case "All":
                    response.Append($"{GetResponseForDistributionAllGameEvent(definition, state, depth)}");
                    break;

                default:
                    response.AppendLine($"\"{Name}\" has an invalid Distribution");
                    break;
            }

            if (response.Length == 0)
            {
                response.AppendLine("Something went wrong");
            }
            return response.ToString();
        }
    }
}
