using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Generic.Dictionary<string, GameBuilderBot.Models.Outcome>;

namespace GameBuilderBot.Models
{
    public class Choice
    {
        public string Name { get; set; }
        public string Distribution { get; set; }
        public string Text { get; set; }
        public bool IsPrimary { get; set; }
        public string Description { get; set; }

        public Dictionary<string, Outcome> outcomeMap = new Dictionary<string, Outcome>();

        public string[] PossibleOutcomes;

        public Choice(ChoiceIngest c)
        {
            Name = c.Name;
            Distribution = c.Distribution;
            Text = c.Text;
            IsPrimary = c.IsPrimary;
            Description = c.Description;

            foreach (OutcomeIngest o in c.Outcomes)
            {
                outcomeMap[o.Name] = new Outcome(o);
            }

        }

        internal string GetSummary()
        {
            return GetSummary(0);
        }

        internal string GetSummary(int depth)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Indent(Name, depth));
            foreach (Outcome o in outcomeMap.Values)
            {
                sb.Append(o.GetSummary(depth));
            }

            return sb.ToString();
        }

        protected string Indent(string s, int depth)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; depth > 0 && i < depth; i++)
            {
                sb.Append("    ");
            }

            return sb.Append("++").Append(s).Append("++").ToString();
        }

        public Outcome GetOutcome(string name)
        {
            return outcomeMap[name];
        }

        public KeyCollection GetOutcomesAsKeys()
        {
            return outcomeMap.Keys;
        }

        public void Complete()
        {
            if (Text == null)
            {
                Text = Name;
            }

            List<string> possibleOutcomes = new List<string>();

            foreach (Outcome o in outcomeMap.Values)
            {
                int count = o.Weight;
                for (int i = 0; i < count; i++)
                {
                    possibleOutcomes.Add(o.Name);
                }

                if (o.Text == null) o.Text = o.Name;
            }

            PossibleOutcomes = possibleOutcomes.ToArray();
        }


    }
}
