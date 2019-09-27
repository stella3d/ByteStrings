using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace ByteStrings
{
    [BurstCompile]
    public struct SingleStringSearchJobInt4 : IJob
    {
        [ReadOnly] public Int4String SearchFor;

        [ReadOnly] public NativeArray<int4> EncodedBufferStrings;
        [ReadOnly] public NativeArray<int> Indices;

        [WriteOnly] public NativeArray<int> FoundIndex;

        public SingleStringSearchJobInt4(Int4String searchFor, Int4StringBuffer buffer, NativeArray<int> output)
        {
            if(output.Length != 1)
                Debug.LogWarning("Please provide a NativeArray<int> of length 1 for the output parameter");
            
            SearchFor = searchFor;
            EncodedBufferStrings = buffer.Data;
            Indices = buffer.Indices;
            FoundIndex = output;
        }

        public void Execute()
        {
            FoundIndex[0] = Search.FindString(ref SearchFor, ref EncodedBufferStrings, ref Indices);
        }
    }
}