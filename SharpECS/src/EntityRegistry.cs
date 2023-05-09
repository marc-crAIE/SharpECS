using SharpECS.Internal;
using SharpECS.Internal.Extensions;
using SharpECS.Internal.Messages;
using System.Reflection;

namespace SharpECS
{
    public sealed class EntityRegistry : IDisposable
    {
        // Used internally for referencing all created registries
        internal static EntityRegistry[] Registries = new EntityRegistry[0];
        internal static UShortDispenser RegistryIDDispenser = new UShortDispenser(1);

        public ushort ID { get; init; }

        private UIntDispenser EntityIDDispenser = new UIntDispenser(1);
        internal Entity[] Entities = new Entity[0];

        #region Constructors

        public EntityRegistry()
        {
            ID = RegistryIDDispenser.GetFree();
            ArrayExtension.EnsureLength(ref Registries, ID);
            Registries[ID] = this;

            Messenger<EntityDisposedMessage>.Subscribe(ID, OnEntityDisposed);

            Messenger.Send(ID, new RegistryCreatedMessage(ID));
        }

        #endregion

        #region General Functions

        public EntityQuery GetEntities() => new EntityQuery(this);

        #endregion

        #region Entity Management Functions

        /// <summary>
        /// Creates a new entity in the registry
        /// </summary>
        /// <returns>The newly created entity identifier</returns>
        public Entity Create()
        {
            uint entityID = EntityIDDispenser.GetFree();
            // Could be an issue if somehow we exceed the max number for a 32-bit integer
            ArrayExtension.EnsureLength(ref Entities, (int)entityID);
            Entities[entityID] = new Entity(ID, entityID);
            return Entities[entityID];
        }

        /// <summary>
        /// Disposes and removes an entity from the registry
        /// <remarks>
        ///     <para>
        ///     This function returns true or false if the entity identifier was removed.
        ///     If it returns false, that means the entity identifier either never existed or has
        ///     already been removed.
        ///     </para>
        /// </remarks>
        /// </summary>
        /// <param name="entity">The entity identifier to be removed</param>
        /// <returns>True if the entity identifier was removed</returns>
        public bool Destroy(Entity entity)
        {
            if (!Valid(entity))
                return false;
            entity.Dispose();
            return true;
        }

        /// <summary>
        /// Dispose and clear all entities in the registry
        /// </summary>
        public void Clear()
        {
            foreach (Entity entity in Entities)
                entity.Dispose();

            Array.Clear(Entities);
            Array.Resize(ref Entities, 0);
        }

        /// <summary>
        /// Copies an entity from the registry to another registry
        /// </summary>
        /// <param name="entity">The entity identifier to copy from</param>
        /// <param name="toRegistry">The registry to copy the entity to</param>
        /// <returns>The copied entity in the other registry</returns>
        public Entity CopyTo(Entity entity, EntityRegistry toRegistry)
        {
            if (!Valid(entity)) return 0;

            Entity copy = toRegistry.Create();
            Messenger.Send(0, new ComponentCopyMessage(entity, copy));
            return copy;
        }

        /// <summary>
        /// Check if an entity identifier is valid or not
        /// </summary>
        /// <param name="entity">The entity identifier to check</param>
        /// <returns>True if the entity identifier is valid</returns>
        public bool Valid(Entity entity)
        {
            return entity < Entities.Length && Entities[entity] == entity;
        }

        /// <summary>/
        /// Returns true or false depending on if the registry is empty
        /// </summary>
        /// <returns>True if the registry is empty</returns>
        public bool Empty()
        {
            return Entities.Length == 0;
        }

        #endregion

        #region Component Functions

        public ref T Add<T>(Entity entity, in T value)
        {
            if (!Valid(entity))
                throw new IndexOutOfRangeException("Entity does not exist");

            return ref ComponentManager<T>.GetOrCreate(ID).Set(entity, value);
        }

        /// <summary>
        /// Adds or replaces a component to an entity
        /// </summary>
        /// <typeparam name="T">The component type</typeparam>
        /// <param name="entity">The entity identifier</param>
        /// <param name="args">Constructor arguments for the component type</param>
        /// <returns>The component</returns>
        /// <exception cref="IndexOutOfRangeException">Occurs if the entity identifier is not valid</exception>
        /// <exception cref="ArgumentException">Occurs if the component constructor arguments are not valid</exception>
        public unsafe ref T Emplace<T>(Entity entity, params object[] args)
        {
            if (!Valid(entity))
                throw new IndexOutOfRangeException("Entity does not exist");

            Type type = typeof(T);
            Type[] argTypes = new Type[args.Length];
            for (int i = 0; i < argTypes.Length; i++)
                argTypes[i] = args[i].GetType();

            ConstructorInfo? ctor = type.GetConstructor(argTypes);
            if (ctor == null)
                throw new ArgumentException($"Invalid arguments for component \"{type}\"");
            T component = (T)ctor.Invoke(args);

            return ref Add(entity, component);
        }

        /// <summary>
        /// Removes a component from an entity
        /// </summary>
        /// <typeparam name="T">The component type to remove</typeparam>
        /// <param name="entity">The entity identifier</param>
        /// <returns>True if removed</returns>
        /// <exception cref="IndexOutOfRangeException">Occurs if the entity does not exist</exception>
        public bool Remove<T>(Entity entity)
        {
            if (!Valid(entity))
                throw new IndexOutOfRangeException("Entity does not exist");

            if (!Has<T>(entity))
                return false;
            ComponentManager<T>.Get(ID).Remove(entity);
            return true;
        }

        /// <summary>
        /// Gets a component from the entity identifier
        /// </summary>
        /// <typeparam name="T">The component type to get</typeparam>
        /// <param name="entity">The entity identifier</param>
        /// <returns>The component from the entity identifier</returns>
        /// <exception cref="InvalidOperationException">Occurs if the entity does not exist or the specified component is not attached</exception>
        public unsafe ref T Get<T>(Entity entity)
        {
            if (!Has<T>(entity))
                throw new InvalidOperationException($"Entity does not exist or does not contain component \"{typeof(T)}\"");
            return ref ComponentManager<T>.Get(ID).Get(entity);
        }

        /// <summary>
        /// Check if an entity identifier has a component
        /// </summary>
        /// <typeparam name="T">The component to check for</typeparam>
        /// <param name="entity">The entity identifier</param>
        /// <returns>True if the component exists, false if the identifier is invalid or the component does not exist</returns>
        public bool Has<T>(Entity entity)
        {
            if (!Valid(entity))
                return false;
            return ComponentManager<T>.Get(ID).Has(entity);
        }

        #endregion

        #region Callbacks

        private void OnEntityDisposed(in EntityDisposedMessage message)
        {
            EntityIDDispenser.Release(message.EntityID);
            ArrayExtension.RemoveAtIndex(ref Entities, (int)message.EntityID);
        }

        #endregion

        #region IDisposeable

        public void Dispose()
        {
            Messenger.Send(0, new RegistryDisposedMessage(ID));
            foreach (Entity entity in Entities)
                entity.Dispose();
            ArrayExtension.RemoveAtIndex(ref Registries, ID);
        }

        #endregion

        #region Object

        public override string ToString() => $"EntityRegistry({ID})";

        #endregion
    }
}