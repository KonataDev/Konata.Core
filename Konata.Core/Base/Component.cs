using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    public abstract class Component:BaseObject
    {
        public Entity Entity { get; set; }

        public T GetEntity<T>()
            where T : Entity
        {
            return (T)this.Entity;
        }

        protected Component()
            : base() { }

        protected Component(long id)
            : base() { }

        /// <summary>
        /// 获取当前组件绑定的实体上其他的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>()
            where T : Component
        {
            return this.Entity?.GetComponent<T>();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;

            long currentid = this.Id;
            base.Dispose();
            Root.Instance.RemoveComponent(currentid);

            this.Entity?.RemoveComponent(this.GetType());
            this.Entity = null;
        }
    }
}
