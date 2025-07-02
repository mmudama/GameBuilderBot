using GameBuilderBot.Services;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameBuilderBot.ExpressionHandling
{
    public class MathExpression
    {
        private readonly string RawExpression;
        private readonly Dictionary<string, GameBuilderBot.Models.Field> Fields;
        private static readonly Regex OperandRegex = new Regex(@"(\+|\-|\*|\/)", RegexOptions.Compiled);

        private static readonly char[] SupportedOperators = { '+', '-', '*', '/' };

        public MathExpression(string rawexpression, Dictionary<string, GameBuilderBot.Models.Field> fields)
        {
            RawExpression = rawexpression;
            Fields = fields;
        }

        protected object IntegerEvaluate()
        {
            string expression = RawExpression;

            foreach (var field in Fields)
            {
                if (expression.Contains(field.Key))
                {
                    expression = expression.Replace(field.Key, field.Value.Value.ToString());
                }
            }


            // Discord will send multiple terms in this way
            // But currently !set doesn't recognize this anyway
            string[] parts = expression.Split('#');
            expression = string.Join(" ", parts);

            return GameBuilderBot.Services.DiceRollService.Roll(expression);
        }


        public bool TryEvaluate(out object result)
        {
            try
            {
                result = Evaluate();
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        public object Evaluate()
        {
            object result = null;
            try
            {
                result = IntegerEvaluate();
            }
            catch (Exception)
            {
                result = ComplexEvaluate(RawExpression);
            }
            return result;
        }

        protected object ComplexEvaluate(string Expression)
        {


            //Split apart based on operators and then perform each operation in order.
            string[] operands = OperandRegex.Split(Expression);

            object intermediateValue = null;
            object currentOperand;
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
                else if (DiceRollService.TryRoll(operands[currentoperandindex], out int roll))
                {
                    operands[currentoperandindex] = roll.ToString();
                }

                currentOperand = FindOperandType(operands, currentoperandindex);

                if (currentoperandindex == 0)
                {
                    intermediateValue = currentOperand;
                }
                else
                {
                    switch (operation)
                    {
                        case "add": intermediateValue = ValueAdd(intermediateValue, currentOperand); break;
                        case "subtract": intermediateValue = ValueSubtract(intermediateValue, currentOperand); break;
                        case "divide": intermediateValue = ValueDivide(intermediateValue, currentOperand); break;
                        case "multiply": intermediateValue = ValueMultiply(intermediateValue, currentOperand); break;
                        default:
                            string msg = "Invalid operation " + operation + " processing expression: " + Expression;
                            throw new System.Exception(msg);
                    }
                }
            }

            return intermediateValue; //Placeholder
        }

        private object FindOperandType(string[] operands, int currentoperandindex)
        {
            object currentOperand;
            //Convert the operand from a string to a type if possible
            if (int.TryParse(operands[currentoperandindex], out int CurrentOperand_int))
            {
                currentOperand = CurrentOperand_int;
            }
            else if (TryStringToDateTime(operands[currentoperandindex], out DateTime CurrentOperand_datetime))
            {
                currentOperand = CurrentOperand_datetime;
            }
            else
            {
                currentOperand = operands[currentoperandindex];
            }

            return currentOperand;
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
