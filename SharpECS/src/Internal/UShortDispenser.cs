using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.src.Internal
{
    internal class UShortDispenser
    {
        private readonly ConcurrentStack<ushort> FreeUShorts = new ConcurrentStack<ushort>();
        private ushort LastUShort;

        #region Constructor

        public UShortDispenser(ushort startUShort)
        {
            LastUShort = startUShort;
        }

        #endregion

        #region General Functions

        public ushort GetFree()
        {
            if (!FreeUShorts.TryPop(out ushort freeUShort))
            {
                freeUShort = LastUShort++;
            }
            return freeUShort;
        }

        public void Release(ushort freeUShort) => FreeUShorts.Push(freeUShort);

        #endregion
    }
}
