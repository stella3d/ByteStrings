using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace ByteStrings
{
    public struct Int4String : IDisposable, IEquatable<Int4String>
    {
        public readonly NativeArray<int4> IntBytes;

        public readonly int TrailingByteCount;
        
        static readonly byte[] k_TempTrailingBytes = new byte[16];
        static readonly int[] k_TempTrailingInts = new int[4];

        const int elementByteCount = 16;
        
        public Int4String(string source, Allocator allocator = Allocator.Persistent)
        {
            var bytes = Encoding.UTF8.GetBytes(source);
            this = new Int4String(bytes, allocator);
        }
        
        public Int4String(NativeArray<int4> source, int trailingByteCount)
        {
            IntBytes = source;
            TrailingByteCount = trailingByteCount;
        }
        
        // TODO - probably can replace some of this with the unsafe cast + trailing handling?
        public Int4String(byte[] bytes, Allocator allocator = Allocator.Persistent)
        {
            var remainder = bytes.Length % elementByteCount;
            TrailingByteCount = remainder == 0 ? 0 : elementByteCount - remainder;
            var alignedCount = bytes.Length + TrailingByteCount;
            var elementCount = alignedCount / elementByteCount;
            IntBytes = new NativeArray<int4>(elementCount, allocator);
            
            var intOutputIndex = 0;
            if (remainder == 0)
            {
                for (var i = 0; i < alignedCount - 15; i += elementByteCount)
                {
                    var x = BitConverter.ToInt32(bytes, i);
                    var y = BitConverter.ToInt32(bytes, i + 4);
                    var z = BitConverter.ToInt32(bytes, i + 8);
                    var w = BitConverter.ToInt32(bytes, i + 12);

                    IntBytes[intOutputIndex] = new int4(x, y, z, w);
                    intOutputIndex++;
                }
            }
            else
            {
                var endIndex = bytes.Length - remainder;
                for (var bi = 0; bi < endIndex; bi += elementByteCount)
                {
                    var x = BitConverter.ToInt32(bytes, bi);
                    var y = BitConverter.ToInt32(bytes, bi + 4);
                    var z = BitConverter.ToInt32(bytes, bi + 8);
                    var w = BitConverter.ToInt32(bytes, bi + 12);
                    
                    IntBytes[intOutputIndex] = new int4(x, y, z, w);
                    intOutputIndex++;
                }

                var trailingIndex = 0;
                for (int i = endIndex; i < bytes.Length; i++)
                {
                    k_TempTrailingBytes[trailingIndex] = bytes[i];
                    trailingIndex++;
                }

                for (int i = trailingIndex; i < elementByteCount; i++)
                    k_TempTrailingBytes[i] = 0;

                const int end = elementByteCount - 3;
                for (int i = 0; i < end; i += 4)
                {
                    k_TempTrailingInts[i / 4] = BitConverter.ToInt32(k_TempTrailingBytes, i);
                }

                var tX = k_TempTrailingInts[0];
                var tY = k_TempTrailingInts[1];
                var tZ = k_TempTrailingInts[2];
                var tW = k_TempTrailingInts[3];
                
                IntBytes[intOutputIndex] = new int4(tX, tY, tZ, tW);
            }
        }

        public override unsafe string ToString()
        {
            var ptr = IntBytes.GetUnsafeReadOnlyPtr();
            var byteReadLength = (IntBytes.Length * elementByteCount) - TrailingByteCount;
            return Encoding.UTF8.GetString((byte*)ptr, byteReadLength);
        }
        
        public void Dispose()
        {
            if(IntBytes.IsCreated) IntBytes.Dispose();
        }

        public bool Equals(Int4String other)
        {
            return IntBytes.Equals(other.IntBytes);
        }
        
        public override bool Equals(object obj)
        {
            return obj is Int4String other && Equals(other);
        }

        public override int GetHashCode()
        {
            return IntBytes.GetHashCode();
        }

        public static bool operator ==(Int4String left, Int4String right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Int4String left, Int4String right)
        {
            return !left.Equals(right);
        }
    }
}

