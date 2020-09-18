using DiscordOregonTrail.Models;
using System;
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
            return new StringBuilder()
                .AppendLine("> !trail\n**Available arguments:**")
                .AppendJoin('\n', Config.choiceMap.Keys)
                .ToString();
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

            response = GetResponse(choice);

            return response;
        }



        public string GetResponse(string choice)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("> Rolling for {0}", choice));

            if (Config.choiceMap.ContainsKey(choice.ToLower()))
            {
                Choice c = Config.choiceMap[choice.ToLower()];

                if (c.Distribution.Equals("Weighted"))
                {
                    int max = c.PossibleOutcomes.Length;
                    int roll = r.Next(0, max);

                    Outcome o = c.GetOutcome(c.PossibleOutcomes[roll]);

                    string outcome = o.Text;

                    if (o.Roll > 0)
                    {
                        int count = r.Next(1, o.Roll + 1);
                        outcome = String.Format(outcome, count);
                    }

                    sb.AppendLine(string.Format("Rolled 1d{0} and got {1}: {2}", max, roll + 1, outcome));

                    if (o.ChildChoice != null)
                    {
                        sb.AppendLine(GetResponse(o.ChildChoice.Name));
                    }
                }
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("Something went wrong");
            }
            return sb.ToString();


        }




    }
}
