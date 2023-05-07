using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Internal
{
    public class ComponentPool<T>
    {
        private ushort RegistryID;
        private T[] Components = new T[0];
        private Dictionary<Entity, int> Mapping = new Dictionary<Entity, int>();

        public ComponentPool(ushort RegistryID)
        {
            this.RegistryID = RegistryID;
        }

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
            Components = Components.Where((c, index) => index != componentIndex).ToArray();
            Mapping.Remove(entity);
            return true;
        }

        public ref T Get(Entity entity)
        {
            if (!Has(entity))
                throw new IndexOutOfRangeException($"Entity does not contain component \"{typeof(T)}\"");

            return ref Components[Mapping[entity]];
        }

        public bool Has(Entity entity)
        {
            return Mapping.ContainsKey(entity);
        }

        public void Clear()
        {
            Mapping.Clear();
            Array.Resize(ref Components, 0);
        }
    }
}
