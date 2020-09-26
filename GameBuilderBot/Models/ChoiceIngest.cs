namespace GameBuilderBot.Models
{
    public class ChoiceIngest
    {
        public string Name { get; set; }
        public string Distribution { get; set; }
        public string Text { get; set; }
        public bool IsPrimary { get; set; }
        public string Description { get; set; }

        public OutcomeIngest[] Outcomes { get; set; }

        public string[] PossibleOutcomes;

        public ChoiceIngest()
        {
        }
    }
}
