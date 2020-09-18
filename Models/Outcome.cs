using System.ComponentModel;

namespace DiscordOregonTrail.Models
{
    public class Outcome
    {

        public string Name { get; set; }
        public int Weight { get; set; }
        public string Text { get; set; }
        public string Choice { get; set; }
        public int Roll { get; set; }

        public Choice _choice;

        public Outcome() { }

        public void Complete()
        {
            
        }

        //public Outcome(string name, int probability) 
        //{
        //    _name = name;
        //    _probability = probability;
        //}
    }
}