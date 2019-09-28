using System.Collections.Generic;
using System.Text;
using Unity.Collections;

namespace ByteStrings
{
    public class Int4StringLookup
    {
        
        internal readonly Dictionary<int, List<string>> m_ByteLengthToStrings = new Dictionary<int, List<string>>();
        
        // we bucket by 4-aligned utf8 byte length
        public readonly Dictionary<int, Int4StringBuffer> Buckets = 
            new Dictionary<int, Int4StringBuffer>();

        int m_BucketAlignment = 16;
        
        public int BucketAlignment
        {
            get => m_BucketAlignment;
            set
            {
                if (value == 16 || value == 8 || value == 4 || value == 2 || value == 1)
                    m_BucketAlignment = value;
            }
        }

        public void AddAll(string[] strings)
        {
            Buckets.Clear();
            m_ByteLengthToStrings.Clear();
            for (int i = 0; i < strings.Length; i++)
            {
                var str = strings[i];
                var byteLength = Encoding.UTF8.GetByteCount(str);
                
                if(!m_ByteLengthToStrings.TryGetValue(byteLength, out var stringList))
                {
                    stringList = new List<string>();
                    m_ByteLengthToStrings[byteLength] = stringList;
                }

                stringList.Add(str);
            }

            foreach (var kvp in m_ByteLengthToStrings)
            {
                var byteLength = kvp.Key;
                var int4StringBuffer = new Int4StringBuffer(kvp.Value.ToArray());
                Buckets.Add(byteLength, int4StringBuffer);
            }
        }

        public void Add(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var bucketKey = AlignToBucket(bytes.Length);
            var alignedByteCount = Utils.Align16(bytes.Length);
                
            if(!m_ByteLengthToStrings.TryGetValue(bucketKey, out var stringList))
            {
                stringList = new List<string>(8);
                m_ByteLengthToStrings[bucketKey] = stringList;
            }

            stringList.Add(input);

            if (!Buckets.TryGetValue(bucketKey, out var buffer))
                buffer = new Int4StringBuffer(8, alignedByteCount);

            buffer.TryAdd(bytes, 0, alignedByteCount / 16);
            Buckets[bucketKey] = buffer;
        }

        int AlignToBucket(int byteLength)
        {
            int remainder = byteLength % m_BucketAlignment;
            return byteLength + m_BucketAlignment - remainder;
        }
        
        public bool TryFind(byte[] bytes, int start, int byteLength)
        {
            var bucketKey = AlignToBucket(byteLength);
            if (!Buckets.TryGetValue(bucketKey, out var bucket))
                return false;

            var i4Str = new Int4String(bytes, start, byteLength, Allocator.Temp);
            var foundIndex = Search.FindString(ref i4Str, ref bucket.Data, ref bucket.Indices);
            i4Str.Dispose();
            return foundIndex != -1;
        }
    }
}