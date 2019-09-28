using System;
using System.Diagnostics.SymbolStore;
using Unity.Burst;
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
            var array = searchFor.IntBytes;
            return FindString(ref array, ref encodedBufferStrings, ref indices);
        }

        /// <summary>
        /// Find a Int4String in a MultiInt4StringBuffer
        /// </summary>
        /// <param name="searchFor">The string to find a match for</param>
        /// <param name="encodedBufferStrings">The shared buffer's bytes</param>
        /// <param name="indices">The shared buffer's string start indices</param>
        /// <returns>The index into the indices buffer that the string was matched at</returns>
        public static int FindString(ref NativeArray<int4> searchFor, ref NativeArray<int4> encodedBufferStrings, 
            ref NativeArray<int> indices)
        {
            var searchArray = searchFor;
            int endIndex = 0;
            for (int i = 1; i < indices.Length; i++)
            {
                var startIndex = indices[i - 1];
                endIndex = indices[i];
                
                // if we're not taking up the same number of 16-byte blocks, strings not equal
                // PERFORMANCE NOTE - unnecessary if using when bucketed by length
                /*
                var length = endIndex - startIndex;
                if (length != searchArray.Length)
                    continue;
                */
                
                var found = true;
                // TODO - for my narrow use case, probably faster to scan in reverse ?
                for (int searchForIndex = 0; searchForIndex < searchArray.Length; searchForIndex++)
                {
                    var bufferIndex = startIndex + searchForIndex;
                    var fromBuffer = encodedBufferStrings[bufferIndex];
                    var fromSearch = searchArray[searchForIndex];
                    
                    // if any of the 4 compared are not equal, the strings are not equal
                    if (math.any(fromBuffer != fromSearch))
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
                
                if (math.any(fromBuffer != fromSearch))
                {
                    lastFound = false;
                    break;
                }
            }

            if (lastFound)
                return indices.Length - 1;

            return -1;
        }

        public class ByteSlice
        {
            public byte[] Buffer;
            public int Start;
            public int Length;
        }

        public static void Queue(ByteSlice utf8Bytes, byte[] copyBuffer, ref int copyStartIndex)
        {
            Buffer.BlockCopy(utf8Bytes.Buffer, utf8Bytes.Start, copyBuffer, copyStartIndex, utf8Bytes.Length);

            var remainder = utf8Bytes.Length % 16;
            var alignedByteCount = Utils.Align16(utf8Bytes.Length);

            // fill with 0s until the next 16-byte alignment point
            for (int i = remainder; i < alignedByteCount; i++)
                copyBuffer[i] = 0;
            
            copyStartIndex += alignedByteCount;
        }
    }
}