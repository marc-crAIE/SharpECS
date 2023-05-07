using SharpECS.Internal;
using SharpECS.Internal.Messages;

namespace SharpECS
{
    public sealed class Entity : IDisposable
    {
        private uint ID;
        private ushort RegistryID;

        #region Constructors

        public Entity(ushort registryID, uint id)
        {
            this.RegistryID = registryID;
            this.ID = id;
        }

        #endregion

        #region Operators

        public static bool operator ==(Entity e1, Entity e2) => e1.ID == e2.ID && e1.RegistryID == e2.RegistryID;
        public static bool operator !=(Entity e1, Entity e2) => !(e1 == e2);

        #endregion

        #region Type Casting

        public static implicit operator uint(Entity entity) => entity.ID;
        public static implicit operator int(Entity entity) => (int)entity.ID;

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
