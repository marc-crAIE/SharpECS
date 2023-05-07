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
    }
}
