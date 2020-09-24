namespace GameBuilderBot.Models
{
    public class OutcomeIngest
    {

        public string Name { get; private set; }
        public int Weight { get; private set; }
        public string Text { get; set; }
        public string Choice { get; private set; }
        public string[] Rolls { get; private set; }

        public OutcomeIngest() { }
    }
}