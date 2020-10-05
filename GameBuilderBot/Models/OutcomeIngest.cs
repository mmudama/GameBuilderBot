using YamlDotNet.Serialization;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// The <seealso cref="Outcome"/> objects from game definition files are deserialized into this object.
    /// Outcomes are members of <seealso cref="GameEventAsString"/>s. This class is a member of <seealso cref="GameEventIngest"/>
    /// <seealso cref="GameEventIngest"/> is a member of <seealso cref="GameFile"/>
    /// </summary>
    /// <seealso cref="GameFile"/>
    public class OutcomeIngest
    {
        public string Name { get; set; }
        public int Weight { get; set; }
        public string Text { get; set; }

        [YamlMember(Alias = "Event")]
        public string GameEventAsString { get; set; }

        public string[] Rolls { get; set; }

        public OutcomeIngest() { }
    }
}
