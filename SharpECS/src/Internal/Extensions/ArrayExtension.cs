using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Internal.Extensions
{
    internal static class ArrayExtension
    {
        public static void EnsureLength<T>(ref T[] array, int index, int maxLength = int.MaxValue)
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
                Array.Resize(ref array, Math.Min(maxLength, newLength));
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
    }
}
