using System;
using Unity.Mathematics;

namespace ByteStrings
{
    public static class ExtensionMethods
    {
        public static byte[] ToManagedBytes(this int4 i4)
        {
            var bytes = new byte[16];
            var outIndex = 0;
            
            var xBytes = BitConverter.GetBytes(i4.x);
            for (var i = 0; i < xBytes.Length; i++)
            {
                bytes[outIndex] = xBytes[i];
                outIndex++;
            }
            
            var yBytes = BitConverter.GetBytes(i4.y);
            for (var i = 0; i < yBytes.Length; i++)
            {
                bytes[outIndex] = yBytes[i];
                outIndex++;
            }
            
            var zBytes = BitConverter.GetBytes(i4.z);
            for (var i = 0; i < zBytes.Length; i++)
            {
                bytes[outIndex] = zBytes[i];
                outIndex++;
            }
            
            var wBytes = BitConverter.GetBytes(i4.w);
            for (var i = 0; i < wBytes.Length; i++)
            {
                bytes[outIndex] = wBytes[i];
                outIndex++;
            }

            return bytes;
        }
        
        public static int4 ToInt4(this byte[] bytes, int start = 0)
        {
            if (bytes.Length != 16)
                return default;

            var x = BitConverter.ToInt32(bytes, start);
            var y = BitConverter.ToInt32(bytes, start + 4);
            var z = BitConverter.ToInt32(bytes, start + 8);
            var w = BitConverter.ToInt32(bytes, start + 12);
            
            return new int4(x, y, z, w);
        }
    }
}