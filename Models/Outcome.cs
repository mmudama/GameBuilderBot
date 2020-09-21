namespace DiscordOregonTrail.Models
{
    public class Outcome
    {

        public string Name { get; private set; }
        public int Weight { get; private set; }
        public string Text { get; set; }
        public string Choice { get; private set; }
        public int[] Rolls { get; private set; }

        public Choice ChildChoice;

        public Outcome() { }

        public void Complete()
        {

        }

    }
}