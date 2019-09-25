using Unity.Collections;

namespace ByteStrings
{
    public static class Search
    {
        /// <summary>
        /// Find a ByteString in a MultiByteStringBuffer
        /// </summary>
        /// <param name="searchFor">The string to find a match for</param>
        /// <param name="bytes">The shared buffer's bytes</param>
        /// <param name="indices">The shared buffer's string start indices</param>
        /// <param name="lengths">The shared buffer's (utf8) string byte lengths</param>
        /// <returns>The index into the indices buffer that the string was matched at</returns>
        public static int FindString(ref ByteString searchFor, ref NativeArray<byte> bytes, 
            ref NativeArray<int> indices, ref NativeArray<int> lengths)
        {
            var searchBytes = searchFor.Bytes;
            for (int i = 0; i < indices.Length; i++)
            {
                var startIndex = indices[i];
                var length = lengths[i];
                var found = true;
                
                // TODO - for my narrow use case, probably faster to scan in reverse ?
                for (int searchForIndex = 0; searchForIndex < length; searchForIndex++)
                {
                    var bufferIndex = startIndex + searchForIndex;
                    if (bytes[bufferIndex] != searchBytes[searchForIndex])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    return i;
            }

            return -1;
        }
    }
}