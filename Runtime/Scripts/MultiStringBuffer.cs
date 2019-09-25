using System;
using System.Text;
using Unity.Collections;

namespace ByteStrings
{
    public class MultiStringBuffer : IDisposable
    {
        static readonly byte[] k_TempIntBytes = new byte[4];
        
        public NativeArray<int> IntBytes;
        
        public NativeArray<int> Indices;
        public NativeArray<int> Lengths;

        public int StringCount;

        public MultiStringBuffer(string[] sources, Allocator allocator = Allocator.Persistent)
        {
            Indices = new NativeArray<int>(sources.Length, allocator);
            Lengths = new NativeArray<int>(sources.Length, allocator);
            
            var totalIntCount = 0;
            for (var i = 0; i < sources.Length; i++)
            {
                var byteCount = Align4(Encoding.UTF8.GetByteCount(sources[i]));
                var intCount = byteCount / 4;
                Indices[i] = totalIntCount;
                Lengths[i] = intCount;
                totalIntCount += intCount;
            }

            IntBytes = new NativeArray<int>(totalIntCount, allocator);
            var intOutputIndex = 0;
            foreach (var s in sources)
            {
                var bytes = Encoding.UTF8.GetBytes(s);
                var endIndex = bytes.Length - 3;
                for (var bi = 0; bi < endIndex; bi += 4)
                {
                    var asInt = BitConverter.ToInt32(bytes, bi);
                    IntBytes[intOutputIndex] = asInt;
                    intOutputIndex++;
                }
            }

            StringCount = sources.Length;
        }

        static int Align4(int count)
        {
            var remainder = count % 4;
            return count + remainder;
        }

        public void Dispose()
        {
            if(IntBytes.IsCreated) IntBytes.Dispose();
            if(Indices.IsCreated) Indices.Dispose();
            if(Lengths.IsCreated) Lengths.Dispose();
        }
    }
    
    public struct MultiByteStringBuffer : IDisposable
    {
        public NativeArray<byte> Bytes;
        
        public NativeArray<int> Indices;
        public NativeArray<int> Lengths;

        public int StringCount;

        public MultiByteStringBuffer(string[] sources, Allocator allocator = Allocator.Persistent)
        {
            Indices = new NativeArray<int>(sources.Length, allocator);
            Lengths = new NativeArray<int>(sources.Length, allocator);
            
            var totalByteCount = 0;
            for (var i = 0; i < sources.Length; i++)
            {
                var byteCount = Encoding.UTF8.GetByteCount(sources[i]);
                Indices[i] = totalByteCount;
                Lengths[i] = byteCount;
                totalByteCount += byteCount;
            }

            Bytes = new NativeArray<byte>(totalByteCount, allocator);
            var outputIndex = 0;
            foreach (var s in sources)
            {
                var bytes = Encoding.UTF8.GetBytes(s);
                foreach (var b in bytes)
                {
                    Bytes[outputIndex] = b;
                    outputIndex++;
                }
            }

            StringCount = sources.Length;
        }
        
        public void Dispose()
        {
            if(Bytes.IsCreated) Bytes.Dispose();
            if(Indices.IsCreated) Indices.Dispose();
            if(Lengths.IsCreated) Lengths.Dispose();
        }
    }
}

