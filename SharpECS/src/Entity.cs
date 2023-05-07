namespace SharpECS
{
    public class Entity
    {
        private uint ID;

        public Entity(uint id) => ID = id;

        public static bool operator ==(Entity e1, Entity e2) => e1.ID == e2.ID;
        public static bool operator !=(Entity e1, Entity e2) => !(e1 == e2);

        public static implicit operator uint(Entity entity) => entity.ID;
        public static implicit operator int(Entity entity) => (int)entity.ID;

        public static implicit operator Entity(uint i) => new Entity(i);
        public static implicit operator Entity(int i) => new Entity((uint)i);

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Entity)) return false;

            Entity? other = obj as Entity;
            return other.ID == this.ID;
        }

        public override int GetHashCode() => ID.GetHashCode();
        public override string ToString() => $"Entity({ID})";
    }
}
