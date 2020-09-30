using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GameBuilderBot.Models.Tests
{
    [TestClass()]
    public class FieldTests
    {
        [TestMethod()]
        public void FieldTest()
        {
            Field intTestField;
            Field stringTestField;
            Field datetimeTestField;

            intTestField = new Field("", "100", "int");
            stringTestField = new Field("", "Jack ran up the hill.", "string");
            datetimeTestField = new Field("", "", "DaTeTime");
        }

        [TestMethod()]
        public void AddToTest()
        {
        }

        [TestMethod()]
        public void AddToDateTimeTest()
        {
            Field datetimeTestField;
            DateTime testvalue;
            datetimeTestField = new Field("", "", "DateTime");

            testvalue = (DateTime)datetimeTestField.Value;
            Assert.AreEqual(testvalue.Ticks, Convert.ToDateTime("1/1/0001 00:00:00 AM").Ticks);

            DateTime ToAdd = new DateTime((long)10000000 * 60 * 65);
            datetimeTestField.AddTo(ToAdd);

            testvalue = (DateTime)datetimeTestField.Value;
            Assert.AreEqual(testvalue.Ticks, Convert.ToDateTime("1/1/0001 1:05:00 AM").Ticks);
        }
    }
}
