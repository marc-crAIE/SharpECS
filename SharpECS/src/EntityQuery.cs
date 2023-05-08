using SharpECS.Internal;
using SharpECS.Internal.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS
{
    public sealed class EntityQuery
    {
        #region Types

        internal enum EitherType
        {
            With,
            Without
        }

        public sealed class EitherQuery
        {
            private readonly EntityQuery Query;
            private readonly EitherType Type;
            private Entity[] Entities;

            #region Constructors

            internal EitherQuery(EntityQuery entityQuery, EitherType type)
            {
                Query = entityQuery;
                Entities = (type == EitherType.With ? new Entity[0] : entityQuery.Entities);
                Type = type;
            }

            #endregion

            #region General Functions

            private EntityQuery Finish() => new EntityQuery(Query.RegistryID, Entities);

            #endregion

            #region Filters

            private EitherQuery OrWith<T>()
            {
                Entity[] poolEntities = ComponentManager<T>.Get(Query.RegistryID).GetEntities();
                Entity[] result = Query.Entities.Intersect(poolEntities).ToArray();
                Entities = Entities.Union(result).ToArray();
                return this;
            }

            private EitherQuery OrWithout<T>()
            {
                Entity[] poolEntities = ComponentManager<T>.Get(Query.RegistryID).GetEntities();
                Entities = Entities.Except(poolEntities).ToArray();
                return this;
            }

            public EitherQuery Or<T>() => Type switch
            {
                EitherType.Without => OrWithout<T>(),
                _ => OrWith<T>()
            };

            public EntityQuery With<T>() => Finish().With<T>();

            public EntityQuery Without<T>() => Finish().Without<T>();

            public EitherQuery WithEither<T>() => Finish().WithEither<T>();

            public EitherQuery WithoutEither<T>() => Finish().WithoutEither<T>();

            #endregion

            #region As Returns

            public Entity[] AsArray() => Entities.ToArray();

            public List<Entity> AsList() => new List<Entity>(Entities.ToArray());

            #endregion

            #region Type Casting

            public static implicit operator EntityQuery(EitherQuery eitherQuery) => eitherQuery.Finish();

            #endregion
        }

        #endregion

        private readonly ushort RegistryID;
        private Entity[] Entities;

        #region Constructors

        internal EntityQuery(ushort registryID, Entity[] entities)
        {
            RegistryID = registryID;
            Entities = entities;
        }

        internal EntityQuery(EntityRegistry registry)
        {
            RegistryID = registry.ID;
            Entities = ArrayExtension.RemoveNulls(registry.Entities);
        }

        #endregion

        #region Filters

        public EntityQuery With<T>()
        {
            Entity[] poolEntities = ComponentManager<T>.Get(RegistryID).GetEntities();
            Entity[] result = Entities.Intersect(poolEntities).ToArray();
            return new EntityQuery(RegistryID, result);
        }

        public EntityQuery Without<T>()
        {
            Entity[] poolEntities = ComponentManager<T>.Get(RegistryID).GetEntities();
            Entity[] result = Entities.Except(poolEntities).ToArray();
            return new EntityQuery(RegistryID, result);
        }

        public EitherQuery WithEither<T>() => new EitherQuery(this, EitherType.With).Or<T>();

        public EitherQuery WithoutEither<T>() => new EitherQuery(this, EitherType.Without).Or<T>();

        #endregion

        #region As Returns

        public Entity[] AsArray() => Entities.ToArray();

        public List<Entity> AsList() => new List<Entity>(Entities.ToArray());

        #endregion
    }
}
