using System;

namespace Konata.Runtime.Base
{
    /// <summary>
    /// 组件初始化工厂
    /// </summary>
    public static class ComponentFactory
    {
        public static Component Create(Type type, Entity entity, long id = 0)
        {
            // Fetch a recycled object from ObjectPool
            var component = (Component)ObjectPool.Instance.Fetch(type);
            {
                if (component == null)
                    return null;

                component.Id = (id != 0 ? id : component.Id);
                component.Parent = entity;
            }

            // Call load method
            ((ILoad)component)?.Load();

            // Append this component
            Root.Instance.AddComponent(component);
            return component;
        }

        public static T Create<T>(long id = 0)
            where T : Component => (T)Create(typeof(T), null, id);

        public static T Create<T>(Entity entity, long id = 0)
            where T : Component => (T)Create(typeof(T), entity, id);
    }
}
