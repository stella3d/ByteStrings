using NUnit.Framework;
using Unity.Mathematics;

namespace ByteStrings.Tests
{
    public static class AssertUtil
    {
        public static void Equals(int4 a, int4 b)
        {
            Assert.AreEqual(a.x, b.x);
            Assert.AreEqual(a.y, b.y);
            Assert.AreEqual(a.z, b.z);
            Assert.AreEqual(a.w, b.w);
        }
    }
}