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

        [TearDown]
        public void AfterEach()
        {
            m_String.Dispose();
            m_ByteStringBuffer.Dispose();
            m_IntString.Dispose();
        }

        [TestCase("Eat the rich")]
        [TestCase("Medicare for all")]
        [TestCase("Health justice now!")]
        public void ByteString_ToString_OutputIsIdentical(string input)
        {
            m_String = new ByteString(input, Allocator.Temp);
            Assert.AreEqual(input, m_String.ToString());
        }
        
        [TestCase("Eat the rich")]
        [TestCase("Medicare for all")]
        [TestCase("Health justice now!")]
        public void IntString_ToString_OutputIsAlmostIdentical(string input)
        {
            m_IntString = new IntString(input, Allocator.Temp);
            Debug.Log($"input - {input}, output - {m_IntString}");
            Assert.AreEqual(input, m_IntString.ToString());
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
        
        [Test]
        public void Search_FindString_InMultiStringBuffer()
        {
            var buffer = new MultiByteStringBuffer(Dragula);
            m_ByteStringBuffer = buffer;

            const int expectedIndex = 1;
            m_String = new ByteString(Dragula[expectedIndex]);

            var index = Search.FindString(ref m_String, ref buffer.Bytes, ref buffer.Indices, ref buffer.Lengths);
            
            Assert.AreEqual(expectedIndex, index);
        }
    }
}
