using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Internal.Extensions
{
    internal static class ArrayExtension
    {
        public static void EnsureLength<T>(ref T[] array, int index)
        {
            if (index >= array.Length)
            {
                int newLength = Math.Max(4, array.Length);
                do
                {
                    newLength *= 2;
                    if (newLength < 0)
                    {
                        newLength = index + 1;
                    }
                }
                while (index >= newLength);
                Array.Resize(ref array, newLength);
            }
        }

        public static void EnsureLength<T>(ref T[] array, int index, in T defaultValue)
        {
            if (index >= array.Length)
            {
                int oldLength = array.Length;

                EnsureLength(ref array, index);
                Fill(ref array, defaultValue, oldLength);
            }
        }

        public static void RemoveAtIndex<T>(ref T[] array, int index)
        {
            if (index >= array.Length)
                throw new IndexOutOfRangeException("Index is outside the bounds of the array!");
            array = array.Where((c, idx) => idx != index).ToArray();
        }

        public static T[] RemoveNulls<T>(T[] array)
        {
            return array.Where(x => x != null).ToArray();
        }

        public static void Fill<T>(ref T[] array, T value, int start = 0)
        {
            for (int i = start; i < array.Length; ++i)
            {
                array[i] = value;
            }
        }
    }
}
