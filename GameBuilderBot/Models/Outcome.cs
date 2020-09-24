using System.Text;

namespace GameBuilderBot.Models
{
    public class Outcome
    {

        public string Name { get; private set; }
        public int Weight { get; private set; }
        public string Text { get; set; }
        public string Choice { get; private set; }
        public string[] Rolls { get; private set; }
        public Field[] Params { get; private set; }

        public Choice ChildChoice;

        public Outcome() { }

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