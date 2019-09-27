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
        public static int FindString(ref ByteString searchFor, ref NativeArray<byte> bytes, ref NativeArray<int> indices)
        {
            var searchBytes = searchFor.Bytes;
            for (int i = 1; i < indices.Length; i++)
            {
                var startIndex = indices[i - 1];
                var endIndex = indices[i];
                
                var found = true;
                var length = endIndex - startIndex;
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
                    return i - 1;
            }

            return -1;
        }
    }
}