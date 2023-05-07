using System.Collections.Concurrent;

namespace SharpECS.Internal
{
    internal sealed class UIntDispenser
    {
        private readonly ConcurrentStack<uint> FreeUInts = new ConcurrentStack<uint>();
        private uint LastUInt;

        #region Constructor

        public UIntDispenser(uint startUInt)
        {
            LastUInt = startUInt;
        }

        #endregion

        #region General Functions

        public uint GetFree()
        {
            if (!FreeUInts.TryPop(out uint freeUInt))
            {
                freeUInt = LastUInt++;
            }
            return freeUInt;
        }

        public void Release(uint freeUInt) => FreeUInts.Push(freeUInt);

        #endregion
    }
}
