using GameBuilderBot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace GameBuilderBot.ExpressionHandling.Tests
{
    [TestClass()]
    public class MathExpressionTests
    {

        string var3_datetime = "1/1/0001 11:00:00 AM";
        Dictionary<string, Field> Fields;
        
        public MathExpressionTests() 
        {
            Fields = new Dictionary<string, Field>
            {
                { "var1_int", new Field("", "100") },
                { "var2_string", new Field("", "Jack ran up the hill.") },
                { "var3_datetime", new Field("", var3_datetime) }
            };
        }

        [TestMethod()]
        public void IntegerTest()
        {
            MathExpression TE;
            object result;

            TE = new MathExpression("var1_int + 10", Fields);
            result = TE.Evaluate();
            Assert.AreEqual("110", result.ToString());

            TE = new MathExpression("var1_int + (2*2)", Fields);
            result = TE.Evaluate();
            Assert.AreEqual("104", result.ToString());

            TE = new MathExpression("50 / 3", Fields);
            result = TE.Evaluate();
            Assert.IsInstanceOfType<int>(result);
            Assert.AreEqual("16", result.ToString(), "Expected integer division to truncate, not round.");

            // TODO failed in Discord! Missing space ... but then succeeded .. huh?
            TE = new MathExpression("50 /3", Fields);
            result = TE.Evaluate();
            Assert.IsInstanceOfType<int>(result);
            Assert.AreEqual("16", result.ToString(), "Expected integer division to truncate, not round.");


            TE = new MathExpression("(2*2 + 20)*100 + 50", Fields);
            result = TE.Evaluate();
            Assert.AreEqual("2450", result.ToString());

            TE = new MathExpression("40*(8 + 3)", Fields);
            result = TE.Evaluate();
            Assert.AreEqual("440", result.ToString());


        }

        [TestMethod()]
        public void StringTest()
        {
            MathExpression TE;
            object result;

            TE = new MathExpression("var2_string+  Jack fell down the hill.", Fields);
            result = TE.Evaluate();
            Assert.AreEqual("Jack ran up the hill.  Jack fell down the hill.", result.ToString());

        }

        [TestMethod()]
        public void DateTimeTest()
        {
            MathExpression TE;
            object result;

            DateTime dateTimeOriginal = DateTime.Parse(var3_datetime);
            DateTime dateTimeCompareSingle = DateTime.Parse(var3_datetime).AddMinutes(30);
            DateTime dateTimeCompareMulti = DateTime.Parse(var3_datetime).AddMinutes(30).AddHours(1);

            // Different runtime environments, causing the comparison test to fail -
            // So, using DateTime functionality to force the same format in comparison
            TE = new MathExpression("var3_datetime + 00:30", Fields);
            result = TE.Evaluate();
            Assert.AreEqual(dateTimeCompareSingle, DateTime.Parse(result.ToString()));


            TE = new MathExpression("var3_datetime + 00:30 + 1:00", Fields);
            result = TE.Evaluate();
            Assert.AreEqual(dateTimeCompareMulti, DateTime.Parse(result.ToString()));


        }

        [TestMethod()]
        public void MultipleTermsTest()
        {

        }

        [TestMethod()]
        public void DiceExpressionTest()
        {            

            //Nondeterministic by nature

            MathExpression TE;
            object result;

            TE = new MathExpression("(5*2) + 1d4", Fields);
            result = TE.Evaluate();
            Assert.IsInstanceOfType<int>(result);
            Assert.IsTrue((int)result > 10 && (int)result <= 14, "Result should be between 11 and 14 inclusive, but was: " + result);

        }
    }
}
