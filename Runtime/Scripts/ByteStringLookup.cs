using System.Collections.Generic;
using System.Text;

namespace ByteStrings
{
    public class StringLookup<T>
    {
        static readonly Dictionary<int, List<string>> k_ByteLengthToStrings = new Dictionary<int, List<string>>();
        
        // we bucket by encoded byte length
        public readonly Dictionary<int, MultiInt4StringBuffer> ByteLengthToBucket = 
            new Dictionary<int, MultiInt4StringBuffer>();

        public void AddAll(string[] strings)
        {
            k_ByteLengthToStrings.Clear();
            for (int i = 0; i < strings.Length; i++)
            {
                var str = strings[i];
                var byteLength = Encoding.UTF8.GetByteCount(str);
                
                if(!k_ByteLengthToStrings.TryGetValue(byteLength, out var stringList))
                {
                    stringList = new List<string>();
                    k_ByteLengthToStrings[byteLength] = stringList;
                }

                stringList.Add(str);
            }

            foreach (var kvp in k_ByteLengthToStrings)
            {
                var byteLength = kvp.Key;
                var int4StringBuffer = new MultiInt4StringBuffer(kvp.Value.ToArray());
                ByteLengthToBucket.Add(byteLength, int4StringBuffer);
            }
        }
    }
}