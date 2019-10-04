using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ByteStrings
{
    public struct ByteString : IDisposable, IEquatable<ByteString>
    {
        public readonly NativeArray<byte> Bytes;

        public static Encoding Encoding = Encoding.ASCII;

        public ByteString(string source, Allocator allocator = Allocator.Persistent)
        {
            var bytes = Encoding.GetBytes(source);
            Bytes = new NativeArray<byte>(bytes, allocator);
        }
        
        public ByteString(byte[] source, int length, int offset = 0, Allocator allocator = Allocator.Persistent)
        {
            Bytes = new NativeArray<byte>(length, allocator);

            // TODO - measure & optimize
            var end = offset + length;
            for (var i = offset; i < end; i++)
                Bytes[i - offset] = source[i];
        }

        public override string ToString()
        {
            unsafe
            {
                var ptr = (byte*) Bytes.GetUnsafeReadOnlyPtr();
                return Encoding.GetString(ptr, Bytes.Length);
            }
        }
        
        public void Dispose()
        {
            if(Bytes.IsCreated) Bytes.Dispose();
        }

        public bool Equals(ByteString other)
        {
            return Bytes.Equals(other.Bytes);
        }
        
        public override bool Equals(object obj)
        {
            return obj is ByteString other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Bytes.GetHashCode();
        }

        public static bool operator ==(ByteString left, ByteString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ByteString left, ByteString right)
        {
            return !left.Equals(right);
        }
    }
}

