using NUnit.Framework;
using Unity.Collections;

namespace ByteStrings.Tests
{
    public class BasicTests
    {
        ByteString m_String;

        [TearDown]
        public void AfterEach()
        {
            m_String.Dispose();
        }

        [TestCase("Eat the rich")]
        [TestCase("Medicare for all")]
        [TestCase("Health justice now!")]
        public void ToString_OutputIsIdentical(string input)
        {
            m_String = new ByteString(input, Allocator.Temp);
            Assert.AreEqual(input, m_String.ToString());
        }
    }
}
