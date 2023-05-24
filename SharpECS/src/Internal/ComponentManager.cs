using SharpECS.Internal.Extensions;
using SharpECS.Internal.Messages;

namespace SharpECS.Internal
{
    internal static class ComponentManager<T>
    {
        private static ComponentPool<T>[] Pools = new ComponentPool<T>[0];

        #region Constructors

        static ComponentManager()
        {
            Messenger<RegistryDisposedMessage>.Subscribe(0, OnRegistryDisposed);
            Messenger<ComponentCopyMessage>.Subscribe(0, OnComponentCopy);

        }

        #endregion

        #region General Functions

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

        public static bool Contains(ushort registryID) => registryID < Pools.Length && Pools[registryID] != null;

        #endregion

        #region Callbacks

        private static void OnRegistryDisposed(in RegistryDisposedMessage message)
        {
            if (!Contains(message.RegistryID)) return;
            ArrayExtension.RemoveAtIndex(ref Pools, message.RegistryID);
        }

        private static void OnComponentCopy(in ComponentCopyMessage message)
        {
            if (!Contains(message.fromEntity.RegistryID)) return;
            ComponentPool<T> fromPool = Pools[message.fromEntity.RegistryID];
            if (fromPool.Has(message.fromEntity))
                GetOrCreate(message.toEntity.RegistryID).Set(message.toEntity, fromPool.Get(message.fromEntity));
        }

        #endregion
    }
}
