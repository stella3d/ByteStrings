using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ByteStrings
{
    public struct ByteString : IDisposable
    {
        public NativeArray<byte> Bytes;

        public ByteString(string source, Allocator allocator = Allocator.Persistent)
        {
            var bytes = Encoding.UTF8.GetBytes(source);
            Bytes = new NativeArray<byte>(bytes, allocator);
        }

        public override string ToString()
        {
            unsafe
            {
                var ptr = (byte*) Bytes.GetUnsafeReadOnlyPtr();
                return Encoding.UTF8.GetString(ptr, Bytes.Length);
            }
        }
        
        public void Dispose()
        {
            if(Bytes.IsCreated) Bytes.Dispose();
        }
    }
}

