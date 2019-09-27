using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace ByteStrings
{
    public static class ArrayUtil
    {
        public static unsafe NativeArray<T> ToNative<T>(ref byte[] bytes, int start, int elementCount, 
            Allocator allocator = Allocator.Persistent)
            where T: struct
        {
            fixed (void* ptr = &bytes[start])
            {
                var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, elementCount, allocator);
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, AtomicSafetyHandle.Create());
                return array;
            }
        }
    }
}