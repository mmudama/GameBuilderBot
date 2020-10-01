namespace GameBuilderBot.Models
{
    /// <summary>
    /// The <seealso cref="Outcome"/> objects from game definition files are deserialized into this object.
    /// Outcomes are members of <seealso cref="Choice"/>s. This class is a member of <seealso cref="ChoiceIngest"/>
    /// <seealso cref="ChoiceIngest"/> is a member of <seealso cref="GameFile"/>
    /// </summary>
    /// <seealso cref="GameFile"/>
    public class OutcomeIngest
    {
        public string Name { get; set; }
        public int Weight { get; set; }
        public string Text { get; set; }
        public string Choice { get; set; }
        public string[] Rolls { get; set; }

        public OutcomeIngest() { }
    }
}
