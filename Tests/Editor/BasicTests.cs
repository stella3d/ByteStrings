﻿using NUnit.Framework;
using Unity.Collections;

namespace ByteStrings.Tests
{
    public class BasicTests
    {
        ByteString m_String;
        MultiByteStringBuffer m_ByteStringBuffer;

        [TearDown]
        public void AfterEach()
        {
            m_String.Dispose();
            m_ByteStringBuffer.Dispose();
        }

        [TestCase("Eat the rich")]
        [TestCase("Medicare for all")]
        [TestCase("Health justice now!")]
        public void ByteString_ToString_OutputIsIdentical(string input)
        {
            m_String = new ByteString(input, Allocator.Temp);
            Assert.AreEqual(input, m_String.ToString());
        }
        
        
        static readonly string[] Dragula =
        {
            "Dig through the ditches and",
            "Burn through the witches and",
            "Slam in the back of my Dragula"
        };

        [Test]
        public void MultiStringBuffer_Constructor()
        {
            m_ByteStringBuffer = new MultiByteStringBuffer(Dragula);
            
            Assert.AreEqual(Dragula.Length, m_ByteStringBuffer.StringCount);
        }
    }
}
