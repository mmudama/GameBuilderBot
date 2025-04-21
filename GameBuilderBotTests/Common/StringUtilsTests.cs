using GameBuilderBot.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameBuilderBotTests.Common
{
    [TestClass()]
    public class StringUtilsTests
    {
        [TestMethod()]
        public void SanitizeForFileNameTest()
        {
            var sanitized = StringUtils.SanitizeForFileName("hi there");
            if (!sanitized.Equals("hi_there"))
            {
                Assert.Fail();
            }

            char c = System.IO.Path.GetInvalidFileNameChars()[0];
            sanitized = StringUtils.SanitizeForFileName(c.ToString());
            if (!sanitized.Equals("_"))
            {
                Assert.Fail();
            }

            string validString = "HiHowAreYou";
            if (!validString.Equals("HiHowAreYou"))
            {
                Assert.Fail();
            }


        }
    }
}