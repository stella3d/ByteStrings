using NUnit.Framework;

namespace ByteStrings.Tests
{
    public class PerformanceTests
    {
        public int StringCount = 1000;
        
        public int MinLength = 20;
        public int MaxLength = 200;
        
        string[] m_SmallerStrings;
        string[] m_Strings;
        
        [OneTimeSetUp]
        public void BeforeAll()
        {
            m_SmallerStrings = TestData.RandomStrings(10, MinLength, MaxLength);
            m_Strings = TestData.RandomStrings(StringCount, MinLength, MaxLength);
        }
    }
}