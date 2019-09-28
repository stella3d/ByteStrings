using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

namespace ByteStrings.Tests
{
    public class StringLookupTests
    {
        Int4StringLookup m_Lookup;
        
        [SetUp]
        public void BeforeAll()
        {
            m_Lookup = new Int4StringLookup();
        }

        [Test]
        public void StringLookup_Add()
        {
            m_Lookup.Add(TestStrings.EatTheRich);
            m_Lookup.Add(TestStrings.M4A);
            m_Lookup.Add(TestStrings.HealthJustice);
            m_Lookup.Add(TestStrings.GetDown);
            m_Lookup.Add(TestStrings.Longer);

            foreach (var line in TestStrings.DragulaLonger)
                m_Lookup.Add(line);
                
            Debug.Log(m_Lookup.ByteLengthToBucket.Count);
        }
    }
}
