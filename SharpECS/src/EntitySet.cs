using SharpECS.Internal;
using SharpECS.Internal.Extensions;

namespace SharpECS
{
    public class EntitySet : Internal.ISet<Entity>
    {
        private readonly ushort RegistryID;

        private int[] Mapping = new int[0];
        private Entity[] Entities = new Entity[0];
        public int Count { get; private set; }

        internal EntitySet(ushort registryID)
        {
            RegistryID = registryID;
        }

        internal EntitySet(ushort registryID, Entity[] entities) : this(registryID)
        {
            foreach (Entity entity in entities)
            {
                Add(entity);
            }
        }

        public void Add(Entity item)
        {
            if (item.RegistryID != RegistryID)
                throw new InvalidOperationException($"Entity (Registry ID: {item.RegistryID}, ID: {(uint)item}) cannot be added to set with Registry ID {RegistryID}");

            if (Contains(item))
                return;

            ArrayExtension.EnsureLength(ref Mapping, item, -1);
            ref int index = ref Mapping[item];
            index = Count++;
            ArrayExtension.EnsureLength(ref Entities, index);
            Entities[index] = item;
        }

        public bool Contains(Entity item) => item < Mapping.Length && Mapping[item] != -1 && Entities[Mapping[item]].RegistryID == item.RegistryID;

        public void Remove(Entity item)
        {
            if (item.RegistryID != RegistryID)
                throw new InvalidOperationException($"Entity (Registry ID: {item.RegistryID}, ID: {(uint)item}) cannot be removed from set with Registry ID {RegistryID}");

            if (item < Mapping.Length)
            {
                ref int index = ref Mapping[item];
                if (index != -1)
                {
                    Count--;
                    if (index != Count)
                    {
                        Entities[index] = Entities[Count];
                        Mapping[Entities[Count]] = index;
                        Entities[Count] = null;
                    }
                    index = -1;
                }
            }
        }
    }
}
