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

        public Outcome[] Outcomes { get; set; }
        protected Dictionary<string, Outcome> outcomeMap;

        public string[] PossibleOutcomes;

        public Choice()
        {
        }

        internal string GetSummary()
        {
            return GetSummary(0);
        }

        internal string GetSummary(int depth)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Indent(Name, depth));
            foreach (Outcome o in Outcomes)
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

            outcomeMap = new Dictionary<string, Outcome>();
            List<string> possibleOutcomes = new List<string>();

            foreach (Outcome o in Outcomes)
            {
                int count = o.Weight;
                for (int i = 0; i < count; i++)
                {
                    possibleOutcomes.Add(o.Name);
                }

                if (o.Text == null) o.Text = o.Name;

                if (o.Name == null)
                {
                    Console.WriteLine(String.Format("**** WARNING: Malformed option in Choice \"{0}\"; skipping", Name));
                }
                else
                {
                    outcomeMap[o.Name] = o;
                }
            }

            PossibleOutcomes = possibleOutcomes.ToArray();
        }


    }
}
