using System.Diagnostics.SymbolStore;
using Unity.Collections;
using Unity.Mathematics;

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
        
        
        /// <summary>
        /// Find a Int4String in a MultiInt4StringBuffer
        /// </summary>
        /// <param name="searchFor">The string to find a match for</param>
        /// <param name="encodedBufferStrings">The shared buffer's bytes</param>
        /// <param name="indices">The shared buffer's string start indices</param>
        /// <returns>The index into the indices buffer that the string was matched at</returns>
        public static int FindString(ref Int4String searchFor, ref NativeArray<int4> encodedBufferStrings, 
            ref NativeArray<int> indices)
        {
            var searchArray = searchFor.IntBytes;
            int endIndex = 0;
            for (int i = 1; i < indices.Length; i++)
            {
                var startIndex = indices[i - 1];
                endIndex = indices[i];
                
                // if we're not taking up the same number of 16-byte blocks, strings not equal
                var length = endIndex - startIndex;
                if (length != searchArray.Length)
                    continue;
                
                var found = true;
                // TODO - for my narrow use case, probably faster to scan in reverse ?
                for (int searchForIndex = 0; searchForIndex < searchArray.Length; searchForIndex++)
                {
                    var bufferIndex = startIndex + searchForIndex;
                    var fromBuffer = encodedBufferStrings[bufferIndex];
                    var fromSearch = searchArray[searchForIndex];
                    
                    // if any of the 4 ints compared are not equal, the strings are not equal
                    if ((fromBuffer != fromSearch).Any())
                    {
                        found = false;
                        break;
                    }
                }
                
                if (found)
                    return i - 1;
            }

            var lastFound = true;
            for (int searchForIndex = 0; searchForIndex < searchArray.Length; searchForIndex++)
            {
                var bufferIndex = endIndex + searchForIndex;
                var fromBuffer = encodedBufferStrings[bufferIndex];
                var fromSearch = searchArray[searchForIndex];

                if ((fromBuffer != fromSearch).Any())
                {
                    lastFound = false;
                    break;
                }
            }

            if (lastFound)
                return indices.Length - 1;

            return -1;
        }
    }
}