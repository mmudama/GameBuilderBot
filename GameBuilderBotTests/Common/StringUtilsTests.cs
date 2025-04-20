using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameBuilderBot.Common.Tests
{
    [TestClass()]
    public class StringUtilsTests
    {
        [TestMethod()]
        public void SanitizeForFileNameTest()
        {
            char c = ' ';
            string invalidString = StringUtils.SanitizeForFileName(" ");
            if (!invalidString.Equals("_"))
            {
                Assert.Fail();
            }

            c = System.IO.Path.GetInvalidFileNameChars()[0];
            invalidString = StringUtils.SanitizeForFileName(c.ToString());
            if (!invalidString.Equals("_"))
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