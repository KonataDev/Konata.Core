using System;
using System.Linq;
using System.Collections.Generic;

namespace Konata.Runtime.Base
{
    public class Entity : BaseObject
    {
        public Entity Parent { get; set; }

        private Dictionary<Type, Component> _componentDict = new Dictionary<Type, Component>();

        protected Entity()
            : base() { }

        protected Entity(long id)
            : base(id) { }

        public override void Dispose()
        {
            if (Id == 0)
            {
                return;
            }
            Parent = null;
            RemoveComponents();
            Root.Instance.RemoveEntity(this);

            base.Dispose();
        }

        public T AddComponent<T>()
            where T : Component, new()
        {
            if (_componentDict.TryGetValue(typeof(T), out Component com))
            {
                return (T)com;
            }

            T ccom = ComponentFactory.Create<T>(this);
            _componentDict.Add(typeof(T), com);
            return ccom;
        }

        public Component AddComponent(Type type)
        {
            if (_componentDict.TryGetValue(type, out Component com))
            {
                return com;
            }

            com = ComponentFactory.Create(type, this);
            if (com == null)
            {
                return null;
            }
            _componentDict.Add(type, com);
            return com;
        }

        //public T AddComponent<T,P>(P p)
        //    where T : Component, new()
        //{
        //    return null;
        //}

        public void RemoveComponent<T>()
            where T : Component
        {
            Type type = typeof(T);
            Component component;
            if (!_componentDict.TryGetValue(type, out component))
                return;
            _componentDict.Remove(type);
            component.Parent = null;
            component.Dispose();
        }

        public void RemoveComponent(Type type)
        {
            if (!_componentDict.TryGetValue(type, out Component component))
            {
                return;
            }
            _componentDict.Remove(type);
            component.Parent = null;
            component.Dispose();
            return;
        }
       

        private void RemoveComponents()
        {
            foreach (Component com in _componentDict.Values)
            {
                com.Parent = null;
                com.Dispose();
            }
            _componentDict.Clear();
        }

        public T GetComponent<T>()
            where T : Component
        {
            if (!_componentDict.TryGetValue(typeof(T), out Component component))
            {
                return default(T);
            }
            return (T)component;
        }

        public Component GetComponent(Type type)
        {
            Component component;
            if (!_componentDict.TryGetValue(type, out component))
            {
                return null;
            }
            return component;
        }

        public Component[] GetComponents()
        {
            return _componentDict.Values.ToArray();
        }
    }
}
