using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordOregonTrail.Models
{
    public class Choice
    {
        public string Name { get; set; }
        public string Distribution { get; set; }
        public string Text { get; set; }
        public Outcome[] Outcomes { get; set; }
        public Dictionary<string, Outcome> outcomeMap;

        public List<string> outcomeNameList = new List<string>();        

        public Choice() {         
        }

        public void Complete()
        {
            if (Text == null)
            {
                Text = Name;
            }

            outcomeMap = new Dictionary<string, Outcome>();

            Console.WriteLine("Loading Choices");

            foreach (Outcome o in Outcomes)
            {
                int count = o.Weight;
                for (int i = 0; i < count; i++)
                {
                    outcomeNameList.Add(o.Name);
                }

                if (o.Text == null)
                {
                    o.Text = o.Name;
                }

                outcomeMap[o.Name] = o;
            }

            Console.WriteLine(String.Format("{0}:\t{1}", Name, outcomeNameList.Count));

        }


    }
}
