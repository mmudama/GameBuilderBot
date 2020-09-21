using DiscordOregonTrail.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace DiscordOregonTrail.Services
{
    public class ResponseService
    {

        protected Random r = new Random();
        Config Config { get; set; }

        public ResponseService(Config config)
        {
            Config = config;

        }

        public string Help()
        {
            var sb = new StringBuilder()
                .AppendLine("> **Help:**");

            foreach (string k in Config.choiceMap.Keys)
            {
                Choice c = Config.choiceMap[k];
                if (c.IsPrimary)
                {                    
                    sb.AppendLine(String.Format("`!game {0}`: {1}", k, c.Description));
                }
            }

            return sb.ToString();
        }

        public string Dump()
        {
            StringBuilder retval = new StringBuilder("> State:\n");

            foreach (string choice in Config.choiceMap.Keys)
            {
                retval.AppendLine(choice);
                foreach (string c in Config.choiceMap[choice].GetOutcomesAsKeys())
                {
                    retval.AppendFormat("\t{0}:\t{1}\n", c, Config.choiceMap[choice].GetOutcome(c).Weight);
                }

            }


            return retval.ToString();
        }

        public string RollEvents(params string[] objects)
        {
            string response = Help();

            if (objects.Length == 0) return response;

            string choice = objects[0];

            response = "\n" + GetResponse(choice);

            return response;
        }

        public string GetResponse(string choice)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine().AppendLine((string.Format("> Rolling for **{0}**", choice)));

            if (Config.choiceMap.ContainsKey(choice.ToLower()))
            {
                Choice c = Config.choiceMap[choice.ToLower()];

                switch (c.Distribution)
                {
                    case "Weighted":
                        sb.Append(GetWeightedResponse(c));
                        break;
                    case "All":
                        sb.Append(GetAllResponse(c));
                        break;
                    default:
                        sb.AppendLine(String.Format("\"{0}\" has an invalid Distribution", choice));
                        break;
                }

            }

            if (sb.Length == 0)
            {
                sb.AppendLine("Something went wrong");
            }
            return sb.ToString();


        }

        private StringBuilder GetAllResponse(Choice c)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Outcome o in c.Outcomes)
            {
                sb.Append(GetOutcomeResponse(o));

                if (o.ChildChoice != null)
                {
                    sb.Append(GetResponse(o.ChildChoice.Name));
                }
            }

            return sb;
        }

        private string GetOutcomeResponse(Outcome o)
        {
            if (o.Roll > 0)
            {
                int count = r.Next(1, o.Roll + 1);
                return String.Format(o.Text, count);
            }
            else
            {
                return o.Text;
            }

        }

        private StringBuilder GetWeightedResponse(Choice c)
        {
            StringBuilder sb = new StringBuilder();

            int max = c.PossibleOutcomes.Length;
            int roll = r.Next(0, max);

            Outcome o = c.GetOutcome(c.PossibleOutcomes[roll]);

            string outcome = GetOutcomeResponse(o);

            sb.AppendLine(string.Format("Rolled 1d{0} and got {1}: **{2}**", max, roll + 1, outcome));

            if (o.ChildChoice != null)
            {
                sb.Append(GetResponse(o.ChildChoice.Name));
            }

            return sb;
        }

        public string GetAllChoices()
        {
            StringBuilder sb = new StringBuilder()
            .AppendLine("> List all possible `!trail` arguments");

            var list = Config.choiceMap.Keys.ToList();
            list.Sort();

            foreach (string choice in list)
            {
                sb.AppendLine(String.Format("`{0}`", choice));
            }

            return sb.ToString();
        }

    }
}
