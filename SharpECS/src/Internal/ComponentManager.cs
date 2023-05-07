using SharpECS.Internal;
using SharpECS.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Internal
{
    internal static class ComponentManager<T>
    {
        private static ComponentPool<T>[] Pools = new ComponentPool<T>[0];

        public static ref ComponentPool<T> Add(ushort registryID)
        {
            ArrayExtension.EnsureLength(ref Pools, registryID);
            
            ref ComponentPool<T> pool = ref Pools[registryID];
            pool ??= new ComponentPool<T>(registryID);

            return ref pool;
        }

        public static ref ComponentPool<T> Get(ushort registryID)
        {
            if (registryID > Pools.Length || Pools[registryID] == null)
                throw new IndexOutOfRangeException("Registry ID does not contain pool");
            return ref Pools[registryID];
        }

        public static ref ComponentPool<T> GetOrCreate(ushort registryID)
        {
            if (registryID > Pools.Length || Pools[registryID] == null)
                return ref Add(registryID);
            return ref Pools[registryID];
        }
    }
}
