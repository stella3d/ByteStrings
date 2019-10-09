using System;
using System.Collections.Generic;

namespace ByteStrings
{
    public unsafe class IntStringLookup<T>
    {
        const int defaultSize = 16;
        
        public readonly Dictionary<ManagedIntString, T> Dictionary;

        // keyed on 4-aligned byte length of bytes to cast
        readonly Dictionary<int, ManagedIntString> CastBuffers;
        
        public IntStringLookup(int initialCapacity = defaultSize)
        {
            Dictionary = new Dictionary<ManagedIntString, T>(initialCapacity);
            CastBuffers = new Dictionary<int, ManagedIntString>(8);
        }

        public void Add(ManagedIntString intStr, T value)
        {
            var alignedByteCount = (intStr.ByteCount + 3) & ~3;
            if (!CastBuffers.TryGetValue(alignedByteCount, out var castBuffer))
            {
                castBuffer = new ManagedIntString(alignedByteCount / 4);
                CastBuffers[alignedByteCount] = castBuffer;
            }
            
            Dictionary.Add(intStr, value);
        }

        public bool TryGetValueFromBytes(byte* ptr, int byteCount, out T value)
        {
            var alignedByteCount = (byteCount + 3) & ~3;
            if (!CastBuffers.TryGetValue(alignedByteCount, out var castBuffer))
            {
                // if there's not already a cast buffer associated with this 4-aligned byte length,
                // that means no string with that aligned byte length has been added, so there can be no match.
                value = default;
                return false;
            }

            castBuffer.SetBytesMemCpy(ptr, byteCount);
            return Dictionary.TryGetValue(castBuffer, out value);
        }

        public void Clear()
        {
            Dictionary.Clear();
            CastBuffers.Clear();
        }
    }
}