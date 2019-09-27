using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Profiling;

namespace ByteStrings.Tests
{
    public class PerformanceTests
    {
        public int StringCount = 1001;
        
        public int MinLength = 20;
        public int MaxLength = 200;
        
        string[] m_SmallerStrings;
        string[] m_Strings;

        Int4StringBuffer m_SmallerBuffer;
        Int4StringBuffer m_Buffer;

        SingleStringSearchJobInt4 m_SmallJob;
        SingleStringSearchJobInt4 m_Job;

        Int4String m_String;
        
        NativeArray<int> m_Result;
        

        [OneTimeSetUp]
        public void BeforeAll()
        {
            m_Result = new NativeArray<int>(1, Allocator.Persistent);
            m_SmallerStrings = TestData.RandomStrings(10, MinLength, MaxLength);
            m_SmallerBuffer = new Int4StringBuffer(m_SmallerStrings);
            
            // this is just to force this job to synchronously compile with Burst before perf test
            var tempInt4String = new Int4String(m_SmallerStrings[0]);
            m_SmallJob = new SingleStringSearchJobInt4(tempInt4String, m_SmallerBuffer, m_Result);
            m_SmallJob.Run();
            
            Search.FindString(ref tempInt4String, ref m_SmallerBuffer.Data, ref m_SmallerBuffer.Indices);
            tempInt4String.Dispose();

            m_Strings = TestData.RandomStringsWithPrefix("/composition", StringCount, MinLength, MaxLength);
            m_Buffer = new Int4StringBuffer(m_Strings);
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            m_Result.Dispose();
            m_Buffer.Dispose();
            m_SmallerBuffer.Dispose();
        }

        [Test]
        public void Search_FindInt4String()
        {
            // second to last in the array, so we can test how long it takes to iterate StringCount times
            var searchForIndex = StringCount - 2; 
            m_String = new Int4String(m_Strings[searchForIndex]);
            
            Profiler.BeginSample("Search_FindInt4String");
            var foundIndex = Search.FindString(ref m_String, ref m_Buffer.Data, ref m_Buffer.Indices);
            Assert.AreEqual(searchForIndex, foundIndex);
            Profiler.EndSample();           

            // jobs make their own profiler label
            m_Job = new SingleStringSearchJobInt4(m_String, m_Buffer, m_Result);
            m_Job.Run();

            Assert.AreEqual(searchForIndex, m_Job.FoundIndex[0]);
            m_String.Dispose();
        }
    }
}