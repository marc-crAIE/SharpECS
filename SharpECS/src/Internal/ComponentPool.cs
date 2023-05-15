using SharpECS.Internal.Extensions;
using SharpECS.Internal.Messages;

namespace SharpECS.Internal
{
    internal struct ComponentLink
    {
        public uint EntityID;

        public ComponentLink(uint entityID)
        { 
            EntityID = entityID; 
        }
    }

    internal class ComponentPool<T>
    {
        private ushort RegistryID;
        private T[] Components = new T[0];
        private ComponentLink[] Links = new ComponentLink[0];
        private int[] Mapping = new int[0];
        private int LastComponentIndex = -1;

        #region Constructors

        public ComponentPool(ushort RegistryID)
        {
            this.RegistryID = RegistryID;

            Messenger<EntityDisposedMessage>.Subscribe(RegistryID, OnEntityDisposed);
        }

        #endregion

        #region General Functions

        public ref T Set(Entity entity, in T component)
        {
            int componentIndex = 0;
            if (Has(entity))
            {
                componentIndex = Mapping[entity];
                Components[componentIndex] = component;
                return ref Components[componentIndex];
            }

            componentIndex = ++LastComponentIndex;

            ArrayExtension.EnsureLength(ref Mapping, entity, -1);
            Mapping[entity] = LastComponentIndex;

            ArrayExtension.EnsureLength(ref Components, LastComponentIndex + 1);
            ArrayExtension.EnsureLength(ref Links, LastComponentIndex + 1);
            Components[componentIndex] = component;
            Links[componentIndex] = new ComponentLink(entity);

            return ref Components[componentIndex];
        }

        public bool Remove(Entity entity)
        {
            if (!Has(entity))
                return false;

            int componentIndex = Mapping[entity];

            Links[componentIndex] = Links[LastComponentIndex];
            Components[componentIndex] = Components[LastComponentIndex];
            Mapping[Links[componentIndex].EntityID] = componentIndex;

            Mapping[entity] = -1;
            Components[LastComponentIndex] = default(T);
            LastComponentIndex--;
            return true;
        }

        public ref T Get(Entity entity)
        {
            if (!Has(entity))
                throw new IndexOutOfRangeException($"Entity does not contain component \"{typeof(T)}\"");

            return ref Components[Mapping[entity]];
        }

        public Entity[] GetEntities()
        {
            Entity[] entities = new Entity[Mapping.Length];
            uint i = 0;
            uint mapIndex = 0;
            while (mapIndex < Mapping.Length)
            {
                if (Mapping[mapIndex] >= 0)
                    entities[i++] = new Entity(RegistryID, mapIndex);
                mapIndex++;
            }
            return ArrayExtension.RemoveNulls(entities);
        }

        public bool Has(Entity entity) => entity < Mapping.Length && Mapping[entity] != -1;

        public void Clear()
        {
            Array.Resize(ref Mapping, 0);
            Array.Resize(ref Components, 0);
        }

        #endregion

        #region Callbacks

        private void OnEntityDisposed(in EntityDisposedMessage message) => Remove(new Entity(RegistryID, message.EntityID));

        #endregion
    }
}
