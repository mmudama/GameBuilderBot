namespace GameBuilderBot.Models
{
    public class ChoiceIngest
    {
        public string Name;
        public string Distribution;
        public string Text;
        public bool IsPrimary;
        public string Description;

        public OutcomeIngest[] Outcomes;

        public string[] PossibleOutcomes;

        public ChoiceIngest()
        {
        }
    }
}
