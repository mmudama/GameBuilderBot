using DiscordOregonTrail.Models;
using System;
using System.Collections.Generic;
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

        public string State()
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

            if (objects.Length == 0 || objects[0].ToLower().Equals("help")) return response;

            string choice = objects[0];

            response = "\n" + GetResponse(choice, 0);

            return response;
        }

        public string GetResponse(string choice, int depth)
        {
            depth++;

            if (depth > 20)
            {
                return "Too much nesting detected. Check your config file. Aborting!";
            }

            StringBuilder sb = new StringBuilder();

            if (Config.choiceMap.ContainsKey(choice.ToLower()))
            {
                sb.AppendLine().AppendLine((string.Format("> Rolling for **{0}**", choice)));
                Choice c = Config.choiceMap[choice.ToLower()];

                switch (c.Distribution)
                {
                    case "Weighted":
                        sb.Append(GetWeightedResponse(c, depth));
                        break;
                    case "All":
                        sb.Append(GetAllResponse(c, depth));
                        break;
                    default:
                        sb.AppendLine(String.Format("\"{0}\" has an invalid Distribution", choice));
                        break;
                }

            }
            else
            {
                sb.AppendLine(String.Format("unrecognized parameter **`{0}`**", choice));
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("Something went wrong");
            }
            return sb.ToString();


        }

        private StringBuilder GetAllResponse(Choice c, int depth)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Outcome o in c.Outcomes)
            {
                sb.AppendLine(GetOutcomeResponse(o));

                if (o.ChildChoice != null)
                {
                    sb.Append(GetResponse(o.ChildChoice.Name, depth));
                }
            }

            return sb;
        }

        private string GetOutcomeResponse(Outcome o)
        {

            string response;

            if (o.Rolls != null && o.Rolls.Length > 0)
            {
                var rolls = new List<int>();
                foreach (int dieSides in o.Rolls)
                {
                    rolls.Add(r.Next(1, dieSides + 1));
                }

                try
                {
                    response = String.Format(o.Text, rolls.Select(x => x.ToString()).ToArray());
                    response = String.Format("**{0}**", response);
                }
                catch (FormatException)
                {
                    response = "**!! Number of rolls specified does not match string format. Check your config file.!!**";
                }
            }
            else
            {
                response = o.Text;
            }

            return response;

        }

        private StringBuilder GetWeightedResponse(Choice c, int depth)
        {
            StringBuilder sb = new StringBuilder();

            int max = c.PossibleOutcomes.Length;
            int roll = r.Next(0, max);

            Outcome o = c.GetOutcome(c.PossibleOutcomes[roll]);

            string outcome = GetOutcomeResponse(o);

            if (max <= 1)
            {
                sb.AppendLine(outcome);
            }
            else if (max == 2)
            {
                sb.AppendLine(string.Format("Flipped a coin and got: **{0}**", outcome));
            }
            else
            {
                sb.AppendLine(string.Format("Rolled 1d{0} and got {1}: **{2}**", max, roll + 1, outcome));
            }

            if (o.ChildChoice != null)
            {
                sb.Append(GetResponse(o.ChildChoice.Name, depth));
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
