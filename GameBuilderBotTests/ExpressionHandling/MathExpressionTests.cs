using GameBuilderBot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            Fields.Add("var1_int", new Field("", "100"));
            Fields.Add("var2_string", new Field("", "Jack ran up the hill."));

            string dateTimeInput = "1/1/0001 11:00:00 AM";
                        Fields.Add("var3_datetime", new Field("", dateTimeInput));

            MathExpression TE;
            object result;

            TE = new MathExpression("var1_int + 10", Fields);
            result = TE.Evaluate();
            Assert.AreEqual("110", result.ToString());

            TE = new MathExpression("var2_string+  Jack fell down the hill.", Fields);
            result = TE.Evaluate();
            Assert.AreEqual("Jack ran up the hill.  Jack fell down the hill.", result.ToString());

            DateTime dateTimeOriginal = DateTime.Parse(dateTimeInput);
            DateTime dateTimeCompare = DateTime.Parse(dateTimeInput).AddMinutes(30);


            // Different environments are using different output formats, so, using DateTime to force the same format in comparison
            TE = new MathExpression("var3_datetime + 00:30", Fields);
            result = TE.Evaluate();
            Assert.AreEqual(dateTimeCompare, DateTime.Parse(result.ToString()));
        }
    }
}
