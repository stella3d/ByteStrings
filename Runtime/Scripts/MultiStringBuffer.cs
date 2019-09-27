using System;
using System.Text;
using Unity.Collections;
using Unity.Mathematics;

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
                var byteCount = Utils.Align4(Encoding.UTF8.GetByteCount(sources[i]));
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

        public int StringCount;

        public MultiByteStringBuffer(string[] sources, Allocator allocator = Allocator.Persistent)
        {
            Indices = new NativeArray<int>(sources.Length, allocator);
            
            var totalByteCount = 0;
            for (var i = 0; i < sources.Length; i++)
            {
                var byteCount = Encoding.UTF8.GetByteCount(sources[i]);
                Indices[i] = totalByteCount;
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
        }
    }
    
    public class MultiInt4StringBuffer : IDisposable
    {
        public NativeArray<int4> Data;
        
        public NativeArray<int> Indices;

        public int StringCount;
        public int ElementCount;

        public int StringCapacity;
        
        public MultiInt4StringBuffer(int stringCapacity, int elementCapacity, Allocator allocator = Allocator.Persistent)
        {
            Indices = new NativeArray<int>(stringCapacity, allocator);
            Data = new NativeArray<int4>(elementCapacity, allocator);
        }
        
        public MultiInt4StringBuffer(string[] sources, Allocator allocator = Allocator.Persistent)
        {
            Indices = new NativeArray<int>(sources.Length, allocator);
            
            var totalInt4Count = 0;
            for (var i = 0; i < sources.Length; i++)
            {
                var byteCount = Utils.Align16(Encoding.UTF8.GetByteCount(sources[i]));
                var int4Count = byteCount / 16;
                Indices[i] = totalInt4Count;
                totalInt4Count += int4Count;
            }

            Data = new NativeArray<int4>(totalInt4Count, allocator);
            var intOutputIndex = 0;
            foreach (var s in sources)
            {
                var int4Str = new Int4String(s, Allocator.Temp);
                foreach (var i4 in int4Str.IntBytes)
                {
                    Data[intOutputIndex] = i4;
                    intOutputIndex++;
                }
                
                int4Str.Dispose();
            }

            StringCount = sources.Length;
        }

        public bool TryAdd(Int4String str)
        {
            if (StringCount >= StringCapacity)
                return false;

            var capacityLeft = Data.Length - ElementCount;
            if (capacityLeft < str.IntBytes.Length)
                return false;

            Indices[StringCount] = ElementCount;
            
            for (var i = 0; i < str.IntBytes.Length; i++)
            {
                Data[ElementCount + i] = str.IntBytes[i];
                ElementCount++;
            }

            return true;
        }

        public void Dispose()
        {
            if(Data.IsCreated) Data.Dispose();
            if(Indices.IsCreated) Indices.Dispose();
        }
    }

    public interface IStringBuffer<T>
    {
        bool TryAdd(T str);
    }
}

