using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameBuilderBot.ExpressionHandling
{
    public class MathExpression
    {
        private string RawExpression;
        private Dictionary<string, GameBuilderBot.Models.Field> Fields;

        private static char[] SupportedOperators = { '+', '-', '*', '/' };

        public MathExpression(string rawexpression, Dictionary<string, GameBuilderBot.Models.Field> fields)
        {
            RawExpression = rawexpression;
            Fields = fields;
        }

        protected object LegacyEvaluate()
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

        public object Evaluate(Boolean UseLegacy = true)
        {
            if (UseLegacy) return LegacyEvaluate();

            return RecursiveEval(RawExpression);
        }

        protected object RecursiveEval(string Expression)
        {
            //If there are parenthesis make a recursive call to evaluate.
            while (Expression.Contains('('))
            {
                int start = Expression.IndexOf('(') + 1;
                int current = start;
                int sublevel = 0;
                int end = 0;
                while (current <= Expression.Length && end <= 0)
                {
                    if (Expression[current].Equals('('))
                    {
                        sublevel++;
                    }
                    else if (Expression[current].Equals(')'))
                    {
                        if (sublevel > 0) sublevel--;
                        else end = current;
                    }
                }
                if (end <= 0)
                {
                    string msg = "Failed to find closing parenthesis in expression: " + Expression;
                    throw new System.Exception(msg);
                }
                Expression = $"{Expression.Substring(0, start - 1)}{RecursiveEval(Expression[start..end])}{Expression.Substring(end + 1, Expression.Length - end)}";
            }

            //Split apart based on operators and then perform each operation in order.
            string[] operands = Regex.Split(Expression, @"([*()\^\/]|(?<!E)[\+\-])");

            object IntermediateValue = null;
            object CurrentOperand;
            string operation = "";

            for (int currentoperandindex = 0; currentoperandindex < operands.Length; currentoperandindex++)
            {
                if ((currentoperandindex + 1) % 2 == 0)
                {
                    switch (operands[currentoperandindex])
                    {
                        case "+": operation = "add"; break;
                        case "-": operation = "subtract"; break;
                        case "/": operation = "divide"; break;
                        case "*": operation = "multiple"; break;
                        default:
                            string msg = "Invalid operand " + operands[currentoperandindex] + " in expression: " + Expression;
                            throw new System.Exception(msg);
                    }
                    currentoperandindex++;
                    if (currentoperandindex > operands.Length)
                    {
                        string msg = "Trailing operand or invalid expression: " + Expression;
                        throw new System.Exception(msg);
                    }
                }

                //If the operand is a variable substitute the value.
                if (Fields.ContainsKey(operands[currentoperandindex].Trim()))
                {
                    operands[currentoperandindex] = Fields[operands[currentoperandindex].Trim()].Value.ToString();
                }

                //Convert the operand from a string to a type if possible
                if (int.TryParse(operands[currentoperandindex], out int CurrentOperand_int))
                {
                    CurrentOperand = CurrentOperand_int;
                }
                else if (TryStringToDateTime(operands[currentoperandindex], out DateTime CurrentOperand_datetime))
                {
                    CurrentOperand = CurrentOperand_datetime;
                }
                else
                {
                    CurrentOperand = operands[currentoperandindex];
                }

                if (currentoperandindex == 0)
                {
                    IntermediateValue = CurrentOperand;
                }
                else
                {
                    switch (operation)
                    {
                        case "add": IntermediateValue = ValueAdd(IntermediateValue, CurrentOperand); break;
                        case "subtract": IntermediateValue = ValueSubtract(IntermediateValue, CurrentOperand); break;
                        case "divide": IntermediateValue = ValueDivide(IntermediateValue, CurrentOperand); break;
                        case "multiply": IntermediateValue = ValueMultiply(IntermediateValue, CurrentOperand); break;
                        default:
                            string msg = "Invalid operation " + operation + " processing expression: " + Expression;
                            throw new System.Exception(msg);
                    }
                }
            }

            return IntermediateValue; //Placeholder
        }

        protected bool TryStringToDateTime(string input, out DateTime output)
        {
            input = input.Trim();

            if (Regex.IsMatch(input, @"^([0-9]+):([0-9]+)$"))  //Handle HH:MM to allow simple hours and minutes math.
            {
                string[] values = input.Split(':');
                long hours = Convert.ToInt64(values[0]);
                long minutes = Convert.ToInt64(values[1]);

                output = new DateTime((long)10000000 * 60 * (hours * 60 + minutes));
                return true;
            }
            else return DateTime.TryParse(input, out output);
        }

        protected object ValueAdd(object first, object second)
        {
            //If either is a string treat as string.
            if ((first.GetType().Equals(typeof(string))) || (second.GetType().Equals(typeof(string))))
            {
                return first.ToString() + second.ToString();
            }

            //If either is a datetime treat as datetime.
            if ((first.GetType().Equals(typeof(DateTime))) || (second.GetType().Equals(typeof(DateTime))))
            {
                return new DateTime(Convert.ToDateTime(first).Ticks + Convert.ToDateTime(second).Ticks);
            }

            //Try as integer
            return Convert.ToInt32(first) + Convert.ToInt32(second);
        }

        protected object ValueSubtract(object first, object second)
        {
            //If either is a string treat as string.
            if ((first.GetType().Equals(typeof(string))) || (second.GetType().Equals(typeof(string))))
            {
                string msg = "Invalid operation, cannot perform subtraction on a string.";
                throw new System.Exception(msg);
            }

            //If either is a datetime treat as datetime.
            if ((first.GetType().Equals(typeof(DateTime))) || (second.GetType().Equals(typeof(DateTime))))
            {
                return new DateTime(Convert.ToDateTime(first).Ticks - Convert.ToDateTime(second).Ticks);
            }

            //Try as integer
            return Convert.ToInt32(first) - Convert.ToInt32(second);
        }

        protected object ValueMultiply(object first, object second)
        {
            //If either is a string treat as string.
            if ((first.GetType().Equals(typeof(string))) || (second.GetType().Equals(typeof(string))))
            {
                string msg = "Invalid operation, cannot perform multiplication on a string.";
                throw new System.Exception(msg);
            }

            //If either is a datetime treat as datetime.
            if ((first.GetType().Equals(typeof(DateTime))) || (second.GetType().Equals(typeof(DateTime))))
            {
                string msg = "Invalid operation, cannot perform multiplication on a DateTime.";
                throw new System.Exception(msg);
            }

            //Try as integer
            return Convert.ToInt32(first) * Convert.ToInt32(second);
        }

        protected object ValueDivide(object first, object second)
        {
            //If either is a string treat as string.
            if ((first.GetType().Equals(typeof(string))) || (second.GetType().Equals(typeof(string))))
            {
                string msg = "Invalid operation, cannot perform division on a string.";
                throw new System.Exception(msg);
            }

            //If either is a datetime treat as datetime.
            if ((first.GetType().Equals(typeof(DateTime))) || (second.GetType().Equals(typeof(DateTime))))
            {
                string msg = "Invalid operation, cannot perform division on a DateTime.";
                throw new System.Exception(msg);
            }

            //Try as integer
            return Convert.ToInt32(first) / Convert.ToInt32(second);
        }
    }
}
