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

        public static T Create<T>(Entity entity)
            where T : Component
        {
            T component = ObjectPool.Instance.Fetch<T>();
            component.Entity = entity;

            return component;
        }
    }
}
