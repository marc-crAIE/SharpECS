using SharpECS.Internal;
using SharpECS.Internal.Messages;

namespace SharpECS
{
    public sealed class Entity : IDisposable
    {
        private uint ID;
        internal readonly ushort RegistryID;

        #region Constructors

        public Entity(ushort registryID, uint id)
        {
            RegistryID = registryID;
            ID = id;

            Messenger.Send(registryID, new EntityCreatedMessage(this));
        }

        #endregion

        #region General Functions

        public bool IsNull() => ID == 0 && RegistryID == 0;

        #endregion

        #region Operators

        public static bool operator ==(Entity e1, Entity e2)
        {
            if (e1 is null || e2 is null)
                return false;
            return e1.ID == e2.ID && e1.RegistryID == e2.RegistryID;
        }
        public static bool operator !=(Entity e1, Entity e2) => !(e1 == e2);

        #endregion

        #region Type Casting

        public static implicit operator uint(Entity entity) => (entity is not null) ? entity.ID : 0;
        public static implicit operator int(Entity entity) => (entity is not null) ? (int)entity.ID : 0;

        public static implicit operator Entity(uint id) => new Entity(0, id);
        public static implicit operator Entity(int id) => new Entity(0, (uint)id);

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Messenger.Send(RegistryID, new EntityDisposedMessage(ID));
        }

        #endregion

        #region Object

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Entity)) return false;

            Entity? other = obj as Entity;
            return other.ID == this.ID && other.RegistryID == this.RegistryID;
        }

        public override int GetHashCode() => ID.GetHashCode();
        public override string ToString() => $"Entity({ID})";

        #endregion
    }
}
