using GameBuilderBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameBuilderBot.Models
{
    public class Config
    {
        public Dictionary<string, Choice> ChoiceMap = new Dictionary<string, Choice>();
        public Dictionary<string, Field> Fields;

        public Config(string fileName)
        {
            var fileContents = File.ReadAllText(fileName);

            var deserializer = new YamlDotNet.Serialization.Deserializer();
            GameFile gameFile = deserializer.Deserialize<GameFile>(fileContents);

            Fields = gameFile.Fields;

            foreach (Choice c in gameFile.Choices)
            {
                c.Complete();
                ChoiceMap[c.Name.ToLower()] = c;
            }

            foreach (Choice c in ChoiceMap.Values)
            {
                foreach (Outcome o in c.Outcomes)
                {
                    if (o.Choice != null)
                    {

                        string key = o.Choice.ToLower();
                        if (ChoiceMap.ContainsKey(key))
                        {
                            o.ChildChoice = ChoiceMap[o.Choice.ToLower()];
                        }
                        else
                        {
                            Console.WriteLine(
                                String.Format("**** WARNING: Outcome \"{0}\" of Choice \"{1}\" specifies child choice \"{2}\"," +
                                " but \"{2}\" is not defined ****", o.Name, c.Name, o.Choice));
                        }
                    }
                }
            }

            foreach (Choice c in ChoiceMap.Values)
            {
                Console.WriteLine(c.GetSummary());
            }

        }


        // Rolls[N] can be an expression (like 1d4)
        // or a reference to a Field. If it starts with "!" and is a recognized
        // Field key, then reroll. Otherwise use the current value. If it's unset (-1),
        // 
        // (ie, -1) (or null using "int?"?) 
        // _config.Roll("!Sunrise") // found in map, prefix "!" means reroll and set
        // _config.Roll("Sunrise") // found in map, retrieve (or roll if unset / < 0) / should I allow negatives?
        // _config.Roll("1d4") // not found in map, so evaluate expression
        // Might want to use something other than "!" since it's meaningful in yaml
        internal int Evaluate(string expression)
        {
            bool evaluate = false;
            string key = expression;
            int result;

            if (expression.StartsWith("!"))
            {
                evaluate = true;
                key = expression.Substring(1);
            }

            if (Fields.ContainsKey(key) && evaluate)
            {
                result = DiceRollService.Roll(Fields[key].Expression);
                Fields[key].Value = result;
            }
            else if (Fields.ContainsKey(key))
            {
                if (Fields[key].Value == null)
                {
                    Fields[key].Value = DiceRollService.Roll(Fields[key].Expression);
                }
                result = (int)Fields[key].Value;
            }
            else
            {
                // assume it's a dice roll expression e.g. "1d6+2"
                result = DiceRollService.Roll(key);
            }

            return result;
        }

    }
}
