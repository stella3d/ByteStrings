using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

namespace ByteStrings.Tests
{
    public class MultiStringBufferTests
    {
        ByteString m_ByteString;
        Int4String m_Int4String;
        MultiByteStringBuffer m_ByteStringBuffer;
        Int4StringBuffer m_Int4StringBuffer;
        
        IntString m_IntString;
        
        static readonly string[] Dragula =
        {
            "Dig through the ditches and",
            "Burn through the witches",
            "I slam in the back of my Dragula"
        };
        
        static readonly string[] DragulaLonger =
        {
            "Dead I am the one, exterminatin' son",
            "Slippin' through the trees, stranglin' the breeze",
            "Dead I am the sky, watchin' angels cry",
            "While they slowly turn, conquering the worm",
            "Dig through the ditches and",
            "Burn through the witches",
            "I slam in the back of my Dragula",
            "Dead I am the pool, spreading from the fool",
            "Weak and want you need, nowhere as you bleed",
            "Dead I am the rat, feast upon the cat",
            "Tender is the fur, dying as you purr"
        };
        
        [TearDown]
        public void AfterEach()
        {
            m_ByteStringBuffer.Dispose();
            m_Int4StringBuffer.Dispose();
            m_ByteString.Dispose();
            m_Int4String.Dispose();
        }

        [Test]
        public void ConstructFromStringArray_ByteStringBuffer()
        {
            m_ByteStringBuffer = new MultiByteStringBuffer(Dragula);
            Assert.AreEqual(Dragula.Length, m_ByteStringBuffer.StringCount);
        }
        
        [Test]
        public void ConstructFromStringArray_Int4StringBuffer()
        {
            m_Int4StringBuffer = new Int4StringBuffer(Dragula);
            Assert.AreEqual(Dragula.Length, m_Int4StringBuffer.StringCount);
        }
        
        [Test]
        public void Search_FindString_InMultiStringBuffer()
        {
            var buffer = new MultiByteStringBuffer(Dragula);
            m_ByteStringBuffer = buffer;

            const int expectedIndex = 1;
            m_ByteString = new ByteString(Dragula[expectedIndex]);

            var index = Search.FindString(ref m_ByteString, ref buffer.Bytes, ref buffer.Indices);
            
            Assert.AreEqual(expectedIndex, index);
        }
        
        [Test]
        public void Search_FindString_LastInMultiInt4StringBuffer()
        {
            var buffer = new Int4StringBuffer(DragulaLonger);
            m_Int4StringBuffer = buffer;

            var expectedIndex = DragulaLonger.Length - 1;
            m_Int4String = new Int4String(DragulaLonger[expectedIndex]);

            var index = Search.FindString(ref m_Int4String, ref buffer.Data, ref buffer.Indices);
            Assert.AreEqual(expectedIndex, index);
        }
        
        [Test]
        public void Search_FindString_InMultiInt4StringBuffer()
        {
            var buffer = new Int4StringBuffer(DragulaLonger);
            m_Int4StringBuffer = buffer;

            var expectedIndex = DragulaLonger.Length - 3;
            m_Int4String = new Int4String(DragulaLonger[expectedIndex]);

            var index = Search.FindString(ref m_Int4String, ref buffer.Data, ref buffer.Indices);
            Assert.AreEqual(expectedIndex, index);
        }
    }
}
