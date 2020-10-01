using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// A Choice object represents a set of possibilities ("Outcomes") available with a virtual "dice roll."
    /// Choices can represent chained dice rolls when one or more of the associated Outcomes results in another Choice.
    /// </summary>
    /// <seealso cref="Outcome"/>
    public class Choice
    {
        /// <summary>
        /// The name field should be short and, ideally, contain only alphabet characters. Any Choice may be
        /// invoked by a user using e.g. `!game [Name]`. User input is treated as case insensitive.
        /// Each Choice Name should map to only one Choice member of a <seealso cref="GameDefinition"/>
        /// or <seealso cref="GameFile"/>
        /// </summary>
        public string Name;

        /// <summary>
        /// Currently "Weighted" or "All"
        /// If the Distribution is "Weighted", the sum of all member Outcomes will be used as the die to roll.
        /// For example, if Outcome A has a weight of 5, B has 12, and C has 3, this Choice would use a virtual
        /// twenty-sided die to determine the outcome. B would be the most likely outcome with a 60% chance of success.
        /// 
        /// If the Distribution is "All", invoking this Choice will result in all Outcomes, not just one..
        /// </summary>
        public string Distribution; // TODO make this an enum

        /// <summary>
        /// TODO is this even used for anything?
        /// </summary>
        public string Text;

        /// <summary>
        /// Indicates whether this Choice should be considered a top-level choice. A user can "roll" any Choice,
        /// but those flagged IsPrimary are considered the most relevant entry points for events. Other
        /// Choices would typically be present only as chained rolls that result from Outcomes.
        /// </summary>
        public bool IsPrimary;

        /// <summary>
        /// One-line description of the Choice, used in help messages etc
        /// </summary>
        public string Description;

        /// <summary>
        /// All Outcomes that may result from this Choice
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
        /// <param name="c"><seealso cref="ChoiceIngest"/>is the deserialized Choice field from a <seealso cref="GameFile"/></param>
        /// 
        public Choice(ChoiceIngest c)
        {
            Name = c.Name;
            Distribution = c.Distribution;
            Text = c.Text;
            IsPrimary = c.IsPrimary;
            Description = c.Description;

            foreach (OutcomeIngest o in c.Outcomes)
            {
                outcomeMap[o.Name] = new Outcome(o);
            }

            if (Text == null)
            {
                Text = Name;
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
    }
}
