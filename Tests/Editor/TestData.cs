using System.Linq;
using UnityEngine;

namespace ByteStrings.Tests
{
    public static class TestData
    {
        public static string[] RandomStrings(int count, int stringLengthMin, int stringLengthMax)
        {
            var strings = new string[count];
            for (int i = 0; i < strings.Length; i++)
                strings[i] = RandomString(stringLengthMin, stringLengthMax);

            return strings;
        }

        public static string RandomString(int minLength, int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ/0123456789";
            
            var length = Random.Range(minLength, maxLength);
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Range(0, s.Length)]).ToArray());
        }
    }
}