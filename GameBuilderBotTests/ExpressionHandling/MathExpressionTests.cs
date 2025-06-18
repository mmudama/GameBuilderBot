using GameBuilderBot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GameBuilderBot.ExpressionHandling.Tests
{
    [TestClass()]
    public class MathExpressionTests
    {
        [TestMethod()]
        public void EvaluateTest()
        {

            Dictionary<string, Field> Fields = new();
            Fields.Add("var1_int", new Field("", "100", "int"));
            Fields.Add("var2_string", new Field("", "Jack ran up the hill.", "string"));
            Fields.Add("var3_datetime", new Field("", "12/12/0001 11:00:00 AM", "datetime"));

            MathExpression TE;
            object result;

            TE = new MathExpression("var1_int + 10", Fields);
            result = TE.Evaluate(false);
            Assert.AreEqual("110", result.ToString());

            TE = new MathExpression("var2_string+  Jack fell down the hill.", Fields);
            result = TE.Evaluate(false);
            Assert.AreEqual("Jack ran up the hill.  Jack fell down the hill.", result.ToString());

            TE = new MathExpression("var3_datetime + 00:30", Fields);
            result = TE.Evaluate(false);
            Assert.AreEqual("12/12/0001 11:30:00 AM", result.ToString());
        }
    }
}
