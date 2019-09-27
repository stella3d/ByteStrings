using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

namespace ByteStrings.Tests
{
    public class BasicTests
    {
        ByteString m_String;
        MultiByteStringBuffer m_ByteStringBuffer;
        
        IntString m_IntString;
        Int4String m_Int4String;
        
        

        [TearDown]
        public void AfterEach()
        {
            m_String.Dispose();
            m_ByteStringBuffer.Dispose();
            m_IntString.Dispose();
        }

        [TestCase(TestStrings.EatTheRich)]
        [TestCase(TestStrings.M4A)]
        [TestCase(TestStrings.HealthJustice)]
        public void ByteString_ToString_OutputIsIdentical(string input)
        {
            m_String = new ByteString(input, Allocator.Temp);
            Assert.AreEqual(input, m_String.ToString());
        }
        
        [TestCase(TestStrings.EatTheRich)]
        [TestCase(TestStrings.M4A)]
        [TestCase(TestStrings.HealthJustice)]
        public void IntString_ToString_OutputIsAlmostIdentical(string input)
        {
            m_IntString = new IntString(input, Allocator.Temp);
            Debug.Log($"input - {input}, output - {m_IntString}");
            Assert.AreEqual(input, m_IntString.ToString());
        }
        
        [TestCase(TestStrings.EatTheRich)]
        [TestCase(TestStrings.M4A)]
        [TestCase(TestStrings.HealthJustice)]
        public void Int4String_ToString_OutputIsAlmostIdentical(string input)
        {
            m_Int4String = new Int4String(input, Allocator.Temp);
            Debug.Log($"input - {input}, output - {m_Int4String}");
            Assert.AreEqual(input, m_Int4String.ToString());
        }
    }

    public static class TestStrings
    {
        public const string EatTheRich = "Eat the rich";
        public const string M4A = "Medicare for all";
        public const string HealthJustice = "Health justice now!";
    }
}
