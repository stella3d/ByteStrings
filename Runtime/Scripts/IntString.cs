using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace ByteStrings
{
    public struct IntString : IDisposable, IEquatable<IntString>
    {
        static readonly byte[] k_TempTrailingBytes = new byte[4];
        
        public readonly NativeArray<int> IntBytes;
        public readonly int TrailingByteCount;
        
        public IntString(string source, Allocator allocator = Allocator.Persistent)
        {
            var bytes = Encoding.UTF8.GetBytes(source);
            Debug.Log($"input int string byte length {bytes.Length}");
            
            var remainder = bytes.Length % 4;
            TrailingByteCount = remainder == 0 ? 0 : 4 - remainder;
            var alignedCount = bytes.Length + remainder;
            var intCount = alignedCount / 4;
            IntBytes = new NativeArray<int>(intCount, allocator);
            
            var intOutputIndex = 0;
            if (remainder == 0)
            {
                for (var bi = 0; bi < alignedCount; bi += 4)
                {
                    IntBytes[intOutputIndex] = BitConverter.ToInt32(bytes, bi);
                    intOutputIndex++;
                }
            }
            else
            {
                var endIndex = bytes.Length - remainder;
                for (var bi = 0; bi < endIndex; bi += 4)
                {
                    IntBytes[intOutputIndex] = BitConverter.ToInt32(bytes, bi);
                    intOutputIndex++;
                }

                var trailingIndex = 0;
                for (int i = endIndex; i < bytes.Length; i++)
                {
                    k_TempTrailingBytes[trailingIndex] = bytes[i];
                    trailingIndex++;
                }

                for (int i = trailingIndex; i < 4; i++)
                {
                    k_TempTrailingBytes[i] = 0;
                }

                IntBytes[intOutputIndex] = BitConverter.ToInt32(k_TempTrailingBytes, 0);
            }
        }

        public override string ToString()
        {
            unsafe
            {
                var ptr = IntBytes.GetUnsafeReadOnlyPtr();
                var byteReadLength = (IntBytes.Length * 4) - TrailingByteCount;
                Debug.Log($"read length: {byteReadLength} , trailing byte count: {TrailingByteCount}");
                return Encoding.UTF8.GetString((byte*)ptr, byteReadLength);
            }
        }
        
        public void Dispose()
        {
            if(IntBytes.IsCreated) IntBytes.Dispose();
        }

        public bool Equals(IntString other)
        {
            return IntBytes.Equals(other.IntBytes);
        }
        
        public override bool Equals(object obj)
        {
            return obj is ByteString other && Equals(other);
        }

        public override int GetHashCode()
        {
            return IntBytes.GetHashCode();
        }

        public static bool operator ==(IntString left, IntString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IntString left, IntString right)
        {
            return !left.Equals(right);
        }
    }
}

