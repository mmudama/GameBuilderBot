using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Generic.Dictionary<string, DiscordOregonTrail.Models.Outcome>;

namespace DiscordOregonTrail.Models
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

        public Outcome GetOutcome(string name)
        {
            return outcomeMap[name];
        }

        public KeyCollection GetOutcomesAsKeys()
        {
            return outcomeMap.Keys;
        }

        public string Complete()
        {

            StringBuilder sb = new StringBuilder();

            if (Text == null)
            {
                Text = Name;
            }

            outcomeMap = new Dictionary<string, Outcome>();
            List<string> possibleOutcomes = new List<string>();

            sb.AppendLine(string.Format("Loading choice \"{0}\"", Name));

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
                    sb.AppendLine(String.Format("**** WARNING: Malformed option in Choice \"{0}\"; skipping", Name));
                }
                else
                {
                    outcomeMap[o.Name] = o;
                    sb.AppendLine(String.Format("*\tOutcome \"{0}\":\tWeight {1}", o.Name, o.Weight));
                }
            }

            PossibleOutcomes = possibleOutcomes.ToArray();

            sb.AppendLine(String.Format("Will roll 1D{1} for \"{0}\"", Name, PossibleOutcomes.Length));
            sb.AppendLine();

            return sb.ToString();

        }


    }
}
