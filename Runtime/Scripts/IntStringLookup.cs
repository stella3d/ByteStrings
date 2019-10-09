using System;
using System.Collections.Generic;

namespace ByteStrings
{
    public unsafe class IntStringLookup<T> : IDisposable
    {
        const int defaultSize = 16;
        
        public readonly Dictionary<ManagedIntString, T> Dictionary;
        readonly Dictionary<string, IntPtr> SourceMapping;

        // keyed on 4-aligned byte length of bytes to cast
        readonly Dictionary<int, ManagedIntString> CastBuffers;
        
        public IntStringLookup(int initialCapacity = defaultSize)
        {
            Dictionary = new Dictionary<ManagedIntString, T>(initialCapacity);
            SourceMapping = new Dictionary<string, IntPtr>(initialCapacity);
            CastBuffers = new Dictionary<int, ManagedIntString>(8);
        }
        
        public void Add(string str, T value)
        {
            if (SourceMapping.ContainsKey(str)) return;

            var iStr = new ManagedIntString(str);
            Add(iStr, value);
            SourceMapping.Add(str, new IntPtr(iStr.OriginalPtr));
        }
        
        public void Remove(string str)
        {
            if (!SourceMapping.TryGetValue(str, out var byteIntPtr)) return;

            var searchStrPtr = (int*) byteIntPtr;
            ManagedIntString toRemove = default;
            foreach (var intStr in Dictionary.Keys)
            {
                if (intStr.OriginalPtr == searchStrPtr)
                {
                    toRemove = intStr;
                    break;
                }
            }

            SourceMapping.Remove(str);
            if (toRemove != default)
            {
                Dictionary.Remove(toRemove);
                toRemove.Dispose();
            }
        }

        public void Add(ManagedIntString intStr, T value)
        {
            var alignedByteCount = (intStr.ByteCount + 3) & ~3;
            if (!CastBuffers.TryGetValue(alignedByteCount, out var castBuffer))
            {
                castBuffer = new ManagedIntString(alignedByteCount / 4);
                CastBuffers.Add(alignedByteCount, castBuffer);
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
        
        public bool TryGetValueFromBytesNoCopy(byte* ptr, int byteCount, out T value)
        {
            var alignedByteCount = (byteCount + 3) & ~3;
            if (!CastBuffers.TryGetValue(alignedByteCount, out var castBuffer))
            {
                // if there's not already a cast buffer associated with this 4-aligned byte length,
                // that means no string with that aligned byte length has been added, so there can be no match.
                value = default;
                return false;
            }

            // override the pointer & count, which should cause
            // both GetHashCode() and Equals() to compare against the new pointer
            castBuffer.Ptr = (int*)ptr;
            castBuffer.ByteCount = byteCount;
            return Dictionary.TryGetValue(castBuffer, out value);
        }

        public void Clear()
        {
            Dictionary.Clear();
            SourceMapping.Clear();
            CastBuffers.Clear();
        }

        public void Dispose()
        {
            foreach (var kvp in Dictionary)
                kvp.Key.Dispose();
            foreach (var kvp in CastBuffers)
                kvp.Value.Dispose();
        }
    }
}