namespace GameBuilderBot.Models
{
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