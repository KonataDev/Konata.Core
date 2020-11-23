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

        public T GetComponent<T>()
            where T : Component
        {
            return this.Entity?.GetComponent<T>();
        }

        public override void Dispose()
        {
            if (this.Id == 0)
            {
                return;
            }
            base.Dispose();

            this.Entity?.RemoveComponent(this.Type);
            this.Entity = null;
        }
    }
}
