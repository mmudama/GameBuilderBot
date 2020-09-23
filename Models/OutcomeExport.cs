namespace DiscordOregonTrail.Models
{
    public class OutcomeExport
    {

        public string Name { get; private set; }
        public int? Weight { get; private set; }
        public string[] Rolls { get; private set; }

        public ChoiceExport ChildChoice { get; private set; }

        public OutcomeExport(Outcome o)
        {
            this.Name = o.Name;
            if (o.Weight > 0)
            {
                this.Weight = o.Weight;
            }
            else
            {
                this.Weight = null;
            }
            if (o.Rolls != null && o.Rolls.Length > 0)
            {
                this.Rolls = o.Rolls;
            }
            if (o.ChildChoice != null)
            {
                this.ChildChoice = new ChoiceExport(o.ChildChoice);
            }
        }
    }
}