using System.Collections.Generic;

namespace GameBuilderBot.Models
{
    public class ChoiceExport
    {
        public string Name { get; set; }
        public string Distribution { get; set; }
        public int? MaxRoll { get; private set; }
        public List<OutcomeExport> Outcomes { get; set; }

        public ChoiceExport(Choice c)
        {
            this.Name = c.Name;
            this.Distribution = c.Distribution;

            if (c.PossibleOutcomes.Length > 0)
            {
                this.MaxRoll = c.PossibleOutcomes.Length;
            }
            else
            {
                this.MaxRoll = null;
            }
            this.Outcomes = new List<OutcomeExport>();

            foreach (Outcome o in c.Outcomes)
            {
                this.Outcomes.Add(new OutcomeExport(o));
            }

        }


    }
}
