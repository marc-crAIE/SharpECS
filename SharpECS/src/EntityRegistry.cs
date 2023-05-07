using SharpECS.Internal;
using System.Reflection;

namespace SharpECS
{
    public class EntityRegistry
    {
        public ushort ID { get; init; }

        private Random Rand = new Random();
        private Dictionary<Entity, uint> Entities = new Dictionary<Entity, uint>();

        #region Constructors

        public EntityRegistry()
        {
            ID = (ushort)Rand.Next(short.MinValue, short.MaxValue);
        }

        #endregion

        #region Entity Management Functions

        /// <summary>
        /// Creates a new entity in the registry
        /// </summary>
        /// <returns>The newly created entity identifier</returns>
        public Entity Create()
        {
            uint id = (uint)Rand.Next(int.MinValue, int.MaxValue);

            while (Entities.ContainsKey(id))
            {
                id = (uint)Rand.Next(int.MinValue, int.MaxValue);
            }
            Entities.Add(id, id);
            return id;
        }

        /// <summary>
        /// Removes an entity from the registry
        /// <remarks>
        ///     <para>
        ///         This function returns true or false if the entity identifier was removed.
        ///         If it returns false, that means the entity identifier either never existed or has
        ///         already been removed.
        ///     </para>
        /// </remarks>
        /// </summary>
        /// <param name="entity">The entity identifier to be removed</param>
        /// <returns>True if the entity identifier was removed</returns>
        public bool Remove(Entity entity)
        {
            return Entities.Remove(entity);
        }

        /// <summary>
        /// Check if an entity identifier is valid or not
        /// </summary>
        /// <param name="entity">The entity identifier to check</param>
        /// <returns>True if the entity identifier is valid</returns>
        public bool Valid(Entity entity)
        {
            return Entities.ContainsKey(entity);
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
        /// <exception cref="InvalidOperationException"></exception>
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
    }
}