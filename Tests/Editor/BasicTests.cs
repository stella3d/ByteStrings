using System.Text;
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
        public void IntString_ToString_OutputIsIdentical(string input)
        {
            m_IntString = new IntString(input, Allocator.Temp);
            Debug.Log($"input - {input}, output - {m_IntString}");
            Assert.AreEqual(input, m_IntString.ToString());
        }
        
        [TestCase(TestStrings.EatTheRich)]
        [TestCase(TestStrings.M4A)]
        [TestCase(TestStrings.HealthJustice)]
        public void Int4String_ToString_OutputIsIdentical(string input)
        {
            m_Int4String = new Int4String(input, Allocator.Temp);
            Debug.Log($"input - {input}, output - {m_Int4String}");
            Assert.AreEqual(input, m_Int4String.ToString());
        }
        
        [TestCase(TestStrings.EatTheRich)]
        [TestCase(TestStrings.M4A)]
        [TestCase(TestStrings.HealthJustice)]
        public void ManagedIntString_ToString_OutputIsIdentical(string input)
        {
            var managedIntString = new ManagedIntString(input);
            Debug.Log($"input - {input}, managed int string output - {managedIntString}");
            Assert.AreEqual(input, managedIntString.ToString());
            managedIntString.Dispose();
        }
        
        
        [TestCase(TestStrings.EatTheRich)]
        [TestCase(TestStrings.M4A)]
        [TestCase(TestStrings.HealthJustice)]
        public void ManagedIntString_SetFromBytes(string input)
        {
            var randomStr = TestData.RandomString(input.Length, input.Length);
            var managedIntString = new ManagedIntString(randomStr);
            Debug.Log($"random string before byte set: {managedIntString}");
            
            var inputAsciiBytes = Encoding.ASCII.GetBytes(input);
            managedIntString.SetBytesUnchecked(inputAsciiBytes, 0, inputAsciiBytes.Length);
            Debug.Log($"input - {input}, managed int string output after byte set- {managedIntString}");
            Assert.AreEqual(input, managedIntString.ToString());
            managedIntString.Dispose();
        }
    }

    public static class TestStrings
    {
        public const string EatTheRich = "Eat the rich";
        public const string M4A = "Medicare for all";
        public const string HealthJustice = "Health justice now!";
        public const string GetDown = "Get down with the sickness";
        
        public const string Longer = "This test string is longer than the first few i wrote for testing.";
        
        public static readonly string[] Dragula =
        {
            "Dig through the ditches and",
            "Burn through the witches",
            "I slam in the back of my Dragula"
        };
        
        public static readonly string[] DragulaLonger =
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
    }
}
