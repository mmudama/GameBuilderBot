using System;
using System.Collections.Generic;
using System.Text;

namespace GameBuilderBot.ExpressionHandling
{
    class MathExpression
    {
        string RawExpression;
        Dictionary<string, GameBuilderBot.Models.Field> Fields;

        public MathExpression(string rawexpression, Dictionary<string, GameBuilderBot.Models.Field> fields)
        {
            RawExpression = rawexpression;
            Fields = fields;
        }

        public object Evaluate()
        {
            //Placeholder logic to get code working with new fields and old int only logic

            string expression = RawExpression;

            string[] parts = expression.Split('#');

            for (int i = 0; i < parts.Length; i++)
            {
                if (Fields.ContainsKey(parts[i]))
                {
                    parts[i] = Fields[parts[i]].Value.ToString();
                }
            }

            expression = string.Join(" ", parts);
            
            return GameBuilderBot.Services.DiceRollService.Roll(expression);
        }
    }
}
