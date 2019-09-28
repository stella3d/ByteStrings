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
            m_ByteStringBuffer = new MultiByteStringBuffer(TestStrings.Dragula);
            Assert.AreEqual(TestStrings.Dragula.Length, m_ByteStringBuffer.StringCount);
        }
        
        [Test]
        public void ConstructFromStringArray_Int4StringBuffer()
        {
            m_Int4StringBuffer = new Int4StringBuffer(TestStrings.Dragula);
            Assert.AreEqual(TestStrings.Dragula.Length, m_Int4StringBuffer.StringCount);
        }
        
        [Test]
        public void Search_FindString_InMultiStringBuffer()
        {
            var buffer = new MultiByteStringBuffer(TestStrings.Dragula);
            m_ByteStringBuffer = buffer;

            const int expectedIndex = 1;
            m_ByteString = new ByteString(TestStrings.Dragula[expectedIndex]);

            var index = Search.FindString(ref m_ByteString, ref buffer.Bytes, ref buffer.Indices);
            
            Assert.AreEqual(expectedIndex, index);
        }
        
        [Test]
        public void Search_FindString_LastInMultiInt4StringBuffer()
        {
            var buffer = new Int4StringBuffer(TestStrings.DragulaLonger);
            m_Int4StringBuffer = buffer;

            var expectedIndex = TestStrings.DragulaLonger.Length - 1;
            m_Int4String = new Int4String(TestStrings.DragulaLonger[expectedIndex]);

            var index = Search.FindString(ref m_Int4String, ref buffer.Data, ref buffer.Indices);
            Assert.AreEqual(expectedIndex, index);
        }
        
        [Test]
        public void Search_FindString_InMultiInt4StringBuffer()
        {
            var buffer = new Int4StringBuffer(TestStrings.DragulaLonger);
            m_Int4StringBuffer = buffer;

            var expectedIndex = TestStrings.DragulaLonger.Length - 3;
            m_Int4String = new Int4String(TestStrings.DragulaLonger[expectedIndex]);

            var index = Search.FindString(ref m_Int4String, ref buffer.Data, ref buffer.Indices);
            Assert.AreEqual(expectedIndex, index);
        }
    }
}
