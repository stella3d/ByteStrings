using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace ByteStrings.Tests
{
    public class ArrayUtilTests
    {
        static byte[] s_SourceBytes;

        static readonly int4 k_SourceInt4 = new int4(420, 69, 666, 20000);

        static int4 s_ManagedFromBytesInt4;
        
        [OneTimeSetUp]
        public void BeforeAll()
        {
            s_SourceBytes = k_SourceInt4.ToManagedBytes();
            s_ManagedFromBytesInt4 = s_SourceBytes.ToInt4();
        }

        [Test]
        public void FromExistingData()
        {
            var array = ArrayUtil.ToNative<int4>(ref s_SourceBytes, 0, 1, Allocator.Temp);
            Assert.AreEqual(1, array.Length);
            AssertUtil.Equals(k_SourceInt4, array[0]);
        }
    }
    
    
}