using System;
using System.Text;
using System.Collections.Generic;

namespace Konata.Runtime.Base
{
    public static class EntityFactory
    {
        public static T Create<T>(long id = 0)
            where T : Entity
        {
            T entity = ObjectPool.Instance.Fetch<T>();
            ILoad ent = entity as ILoad;
            if (ent != null)
            {
                ent.Load();
            }
            if (id != 0)
            {
                entity.Id = id;
            }
            Root.Instance.AddEntity(entity);
            return entity;
        }

        public static Entity Create<T>(Type type, long id = 0)
        {
            Entity entity = (Entity)ObjectPool.Instance.Fetch(type);
            if (entity == null)
            {
                return null;
            }
            ILoad ent = entity as ILoad;
            if (ent != null)
            {
                ent.Load();
            }
            if (id != 0)
            {
                entity.Id = id;
            }
            Root.Instance.AddEntity(entity);
            return entity;
        }
    }
}
