using NUnit.Framework;
using UnityEngine.Profiling;

namespace ByteStrings.Tests
{
    public class PerformanceTests
    {
        public int StringCount = 10001;
        
        public int MinLength = 20;
        public int MaxLength = 200;
        
        string[] m_SmallerStrings;
        string[] m_Strings;

        Int4StringBuffer m_SmallerBuffer;
        Int4StringBuffer m_Buffer;
        
        [OneTimeSetUp]
        public void BeforeAll()
        {
            m_SmallerStrings = TestData.RandomStrings(10, MinLength, MaxLength);
            m_SmallerBuffer = new Int4StringBuffer(m_SmallerStrings);
            
            // this is just to force this method to synchronously compile with Burst before perf test
            var tempInt4String = new Int4String(m_SmallerStrings[0]);
            Search.FindString(ref tempInt4String, ref m_SmallerBuffer.Data, ref m_SmallerBuffer.Indices);
            tempInt4String.Dispose();

            m_Strings = TestData.RandomStrings(StringCount, MinLength, MaxLength);
            m_Buffer = new Int4StringBuffer(m_Strings);
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            m_Buffer.Dispose();
            m_SmallerBuffer.Dispose();
        }

        [Test]
        public void Search_FindInt4String()
        {
            // second to last in the array, so we can test how long it takes to iterate StringCount times
            var searchForIndex = StringCount - 2; 
            var searchFor = new Int4String(m_Strings[searchForIndex]);
            Profiler.BeginSample("Search_FindInt4String");

            Search.FindString(ref searchFor, ref m_Buffer.Data, ref m_Buffer.Indices);
            
            Profiler.EndSample();            
            searchFor.Dispose();
        }
    }
}