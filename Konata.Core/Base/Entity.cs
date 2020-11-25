using System;
using System.Collections.Generic;
using System.Linq;

namespace Konata.Core.Base
{
    public class Entity:BaseObject
    {
        public Entity Parent { get; set; }

        private Dictionary<Type, Component> componentDict = new Dictionary<Type, Component>();

        protected Entity()
            : base() { }

        protected Entity(long id)
            : base(id) { }

        public override void Dispose()
        {
            if (this.Id == 0)
            {
                return;
            }
            this.RemoveComponents();

            base.Dispose();
        }

        public T AddComponent<T>()
            where T : Component, new()
        {
            
            return null;
        }

        public T AddComponent<T,P>(P p)
            where T : Component, new()
        {
            return null;
        }

        public void RemoveComponent<T>()
            where T: Component
        {
            Type type = typeof(T);
            Component component;
            if (!this.componentDict.TryGetValue(type, out component))
                return;
            this.componentDict.Remove(type);
            component.Entity = null;
            component.Dispose();
        }

        public void RemoveComponent(Type type)
        {
            if (!this.componentDict.TryGetValue(type,out Component component))
            {
                return;
            }
            this.componentDict.Remove(type);
            component.Entity = null;
            component.Dispose();
            return;
        }

        private void RemoveComponents()
        {
            foreach (Component com in this.componentDict.Values)
            {
                com.Entity = null;
                com.Dispose();
            }
            this.componentDict.Clear();
        }

        public T GetComponent<T>()
            where T : Component
        {
            if(!this.componentDict.TryGetValue(typeof(T),out Component component))
            {
                return default(T);
            }
            return (T)component;
        }

        public Component GetComponent(Type type)
        {
            Component component;
            if (!this.componentDict.TryGetValue(type, out component))
            {
                return null;
            }
            return component;
        }

        public Component[] GetComponents()
        {
            return this.componentDict.Values.ToArray();
        }
    }
}
