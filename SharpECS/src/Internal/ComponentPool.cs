using SharpECS.Internal.Extensions;
using SharpECS.Internal.Messages;

namespace SharpECS.Internal
{
    internal class ComponentPool<T>
    {
        private ushort RegistryID;
        private T[] Components = new T[0];
        private Dictionary<Entity, int> Mapping = new Dictionary<Entity, int>();

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
            if (Has(entity))
            {
                int componentIndex = Mapping[entity];
                Components[componentIndex] = component;
                return ref Components[componentIndex];
            }

            Array.Resize(ref Components, Components.Length + 1);
            Mapping.Add(entity, Components.Length - 1);
            Components[Components.Length - 1] = component;
            return ref Components[Components.Length - 1];
        }

        public bool Remove(Entity entity)
        {
            if (!Has(entity))
                return false;

            int componentIndex = Mapping[entity];
            ArrayExtension.RemoveAtIndex(ref Components, componentIndex);
            Mapping.Remove(entity);
            return true;
        }

        public ref T Get(Entity entity)
        {
            if (!Has(entity))
                throw new IndexOutOfRangeException($"Entity does not contain component \"{typeof(T)}\"");

            return ref Components[Mapping[entity]];
        }

        public Entity[] GetEntities() => Mapping.Keys.ToArray();

        public bool Has(Entity entity)
        {
            return Mapping.ContainsKey(entity);
        }

        public void Clear()
        {
            Mapping.Clear();
            Array.Resize(ref Components, 0);
        }

        #endregion

        #region Callbacks

        private void OnEntityDisposed(in EntityDisposedMessage message) => Remove(new Entity(RegistryID, message.EntityID));

        #endregion
    }
}
