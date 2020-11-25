using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    /// <summary>
    /// 组件初始化工厂
    /// </summary>
    public static class ComponentFactory
    {

        public static T Create<T>(Entity entity,long id=0)
            where T : Component
        {
            T component = ObjectPool.Instance.Fetch<T>();
            component.Entity = entity;
            ILoad com = component as ILoad;
            if (com != null)
            {
                com.Load();
            }
            if (id != 0)
            {
                component.Id = id;
            }
            Root.Instance.AddComponent(component);
            return component;
        }

        public static Component Create(Type type,Entity entity,long id=0)
        {
            Component component = (Component)ObjectPool.Instance.Fetch(type);
            if (component == null)
            {
                return null;
            }
            component.Entity = entity;
            ILoad com = component as ILoad;
            if (com != null)
            {
                com.Load();
            }
            if (id != 0)
            {
                component.Id = id;
            }
            Root.Instance.AddComponent(component);
            return component;
        }
        public static T Create<T>(long id=0)
            where T : Component
        {
            T component = ObjectPool.Instance.Fetch<T>();
            component.Entity = null;
            ILoad com = component as ILoad;
            if (com != null)
            {
                com.Load();
            }
            if (id != 0)
            {
                component.Id = id;
            }
            Root.Instance.AddComponent(component);
            return component;
        }
    }
}
