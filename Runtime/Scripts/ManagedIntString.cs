using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace ByteStrings
{
    public unsafe struct ManagedIntString : IDisposable, IEquatable<ManagedIntString>
    {
        const int intSize = 4;
        internal static Encoding Encoding { get; } = Encoding.ASCII;
        
        internal readonly int[] Bytes;
        internal int ByteCount;

        GCHandle m_BytesHandle;
        internal int* Ptr;
        
        public ManagedIntString(string source)
        {
            var bytes = Encoding.GetBytes(source);
            var alignedByteCount = (bytes.Length + 3) & ~3;
            ByteCount = bytes.Length;
            Bytes = new int[alignedByteCount / intSize];
            // pin the address of our bytes for the lifetime of this object
            m_BytesHandle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            Ptr = (int*) m_BytesHandle.AddrOfPinnedObject();
            Buffer.BlockCopy(bytes, 0, Bytes, 0, bytes.Length);
            //Debug.Log($"input int string byte length {bytes.Length}, aligned {alignedByteCount}");
        }
        
        internal ManagedIntString(int intCapacity)
        {
            ByteCount = 0;
            Bytes = new int[intCapacity];
            m_BytesHandle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            Ptr = (int*) m_BytesHandle.AddrOfPinnedObject();
        }

        public override string ToString()
        {
            return Encoding.GetString((byte*) Ptr, ByteCount);
        }

        public void SetBytes(byte[] bytes, int offset, int byteLength)
        {
            var alignedByteCount = (byteLength + 3) & ~3;
            if (alignedByteCount / 4 != Bytes.Length)
            {
                Debug.LogError("Tried to set managed int string from bytes, " + 
                               $"but byte length of {byteLength} does not match int length {Bytes.Length}");
                return;
            }

            if (ByteCount != byteLength)
                Bytes[Bytes.Length - 1] = 0;  
            
            ByteCount = byteLength;
            Buffer.BlockCopy(bytes, offset, Bytes, 0, byteLength);
        }

        public void SetBytesUnchecked(byte[] bytes, int offset, int byteLength)
        {
            // if we have more trailing bytes after setting, that means we'd be left with junk.
            // since trailing bytes are always in the last int, just set it to 0 to clear that part before we copy.
            if (ByteCount < byteLength)
                Bytes[Bytes.Length - 1] = 0;  
            
            ByteCount = byteLength;
            Buffer.BlockCopy(bytes, offset, Bytes, 0, byteLength);
        }
        
        public void SetBytesMemCpy(byte* bytes, int byteLength)
        {
            // if we have more trailing bytes after setting, that means we'd be left with junk.
            // since trailing bytes are always in the last int, just set it to 0 to clear that part before we copy.
            if (ByteCount < byteLength)
                Bytes[Bytes.Length - 1] = 0;  
            
            ByteCount = byteLength;
            memcpy(Ptr, bytes, (UIntPtr) byteLength);
        }
        
        public void Dispose()
        {
            m_BytesHandle.Free();
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                // hash the last element with the length
                var length = Bytes.Length;
                return length ^ 397 + Bytes[length - 1];
            }
        }
        
        // comparing bytes using memcmp has shown to be several times faster than any other method i've found
        [DllImport("msvcrt.dll", EntryPoint = "memcmp")]
        static extern int memcmp(void* ptr1, void* ptr2, long count);
        
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        internal static extern IntPtr memcpy(void* dest, void* src, UIntPtr count);

        public bool CompareBytes(void* otherPtr, int count)
        {
            return count == ByteCount && memcmp(Ptr, otherPtr, count) == 0;
        }

        public bool CompareBytes(ManagedIntString other)
        {
            if (other.ByteCount != ByteCount) return false;
            return memcmp(Ptr, other.Ptr, ByteCount) == 0;
        }
        
        public bool Equals(ManagedIntString other)
        {
            if (other.ByteCount != ByteCount) return false;
            return memcmp(Ptr, other.Ptr, ByteCount) == 0;
        }
        
        public static bool Equals(ManagedIntString l, ManagedIntString r)
        {
            return l.ByteCount == r.ByteCount && memcmp(l.Ptr, r.Ptr, r.ByteCount) == 0;
        }

        // fallback for equality checks if we can't use memcmp
        bool ManagedEquals(ManagedIntString other)
        {
            if (Bytes.Length % 2 == 0)
            {
                fixed (int* otherPtr = other.Bytes)
                {
                    for (int i = 0; i < Bytes.Length; i += 2)
                    {
                        if (*(ulong*) (Ptr + i) != *(ulong*) (otherPtr + i)) return false;
                    }
                }
            }
            else
            {
                fixed (int* otherPtr = other.Bytes)
                {
                    for (int i = 0; i < Bytes.Length; i++)
                    {
                        if ((Ptr + i) != (otherPtr + i)) return false;
                    }
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) && Equals((ManagedIntString) obj);
        }

        public static bool operator ==(ManagedIntString l, ManagedIntString r)
        {
            return l.ByteCount == r.ByteCount && memcmp(l.Ptr, r.Ptr, r.ByteCount) == 0;
        }

        public static bool operator !=(ManagedIntString l, ManagedIntString r)
        {
            return !(l.ByteCount == r.ByteCount && memcmp(l.Ptr, r.Ptr, r.ByteCount) == 0);
        }
    }
}

