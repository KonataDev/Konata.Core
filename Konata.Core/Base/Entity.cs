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

        public bool RemoveComponent<T>()
            where T: Component
        {
            return true;
        }

        public bool RemoveComponent(Type type)
        {
            Component component;
            if(!this.componentDict.TryGetValue(type,out component))
            {
                return false;
            }
            this.componentDict.Remove(type);
            
            component.Dispose();
            return true;
        }

        public T GetComponent<T>()
            where T : Component
        {
            Component component;
            if(!this.componentDict.TryGetValue(typeof(T),out component))
            {
                return default(T);
            }
            return (T)component;
        }

        public Component[] GetComponents()
        {
            return this.componentDict.Values.ToArray();
        }
    }
}
