using System;
using System.Collections.Generic;
using System.Text;

namespace GameBuilderBot.Models
{
    public class Outcome
    {

        public string Name { get; private set; }
        public int Weight { get; private set; }
        public string Text { get; set; }
        public string[] Rolls { get; private set; }
        public string Choice;

        public Choice ChildChoice;

        public Outcome(OutcomeIngest o)
        {
            Name = o.Name;
            Weight = o.Weight;
            Text = o.Text;
            Rolls = o.Rolls;
            Choice = o.Choice;
        }

        public void Complete(Dictionary<string, Choice> ChoiceMap)
        {
            if (Choice != null)
            {
                string key = Choice.ToLower();
                if (ChoiceMap.ContainsKey(key))
                {
                    ChildChoice = ChoiceMap[key];
                }
                else
                {
                    Console.WriteLine(
                        string.Format("**** WARNING: Outcome \"{0}\" of Choice \"{1}\" specifies child choice \"{2}\"," +
                        " but \"{2}\" is not defined ****", Name, key, Choice));
                }
            }
        }



        internal string GetSummary(int depth)
        {
            StringBuilder sb = new StringBuilder();
            if (ChildChoice == null)
            {
                sb.AppendLine(Indent(Name, depth));
            }
            else
            {
                sb.AppendLine(Indent(Name + ":", depth));
                sb.Append(ChildChoice.GetSummary(depth + 1));
            }

            return sb.ToString();

        }

        protected string Indent(string s, int depth)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                sb.Append("    ");
            }

            return sb.AppendFormat("* {0} ({1})", s, Weight).ToString();
        }

    }
}