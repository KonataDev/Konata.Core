using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konata.Core.Base;
using Konata.Core.Utils;

namespace Konata.Core
{
    public class Root
    {
        private static Root instance;
        public static Root Instance
        {
            get => instance ?? (instance = new Root());
        }
        
        private Dictionary<long, Component> components = new Dictionary<long, Component>();

        private UnOrderMultiMap<Type, Entity> entities = new UnOrderMultiMap<Type, Entity>();

        private Dictionary<Type, IDisposable> services = new Dictionary<Type, IDisposable>();

        private object comlock = new object();
        private object entlock = new object();
        private object serlock = new object();


        public bool AddComponent(Component component)
        {
            lock (comlock)
            {
                if (this.components.ContainsKey(component.Id))
                {
                    return false;
                }
                this.components.Add(component.Id, component);
                return true;
            }
        }



        public bool AddEntity(Entity entity)
        {
            lock (entlock)
            {
                Type type = entity.GetType();
                if (this.entities.Contains(type, entity))
                {
                    return false;
                }
                this.entities.Add(type, entity);
                return true;
            }
        }


        public void RemoveComponent(long id)
        {
            lock (comlock)
            {
                if (this.components.ContainsKey(id))
                    this.components.Remove(id);
            }
        }

        public void RemoveComponents(Type type,bool onlyremove=true)
        {
            lock (comlock)
            {
                var coms = this.components.Where(u => u.Value.GetType() == type);
                foreach(KeyValuePair<long,Component> c in coms)
                {
                    if (!onlyremove)
                    {
                        c.Value.Dispose();
                    }
                    this.components.Remove(c.Key);

                }
            }
        }
        public void RemoveEntity(Entity entity)
        {
            lock (entlock)
            {
                Type type = entity.GetType();
                if (this.entities.Contains(type, entity))
                {
                    this.entities.Remove(type, entity);
                }
            }
        }

        public void RemoveEntities(Type type,bool onlyremove=true)
        {
            lock (entlock)
            {
                if (this.entities.ContainsKey(type))
                {
                    if (!onlyremove)
                    {
                        IList<Entity> entities = this.entities.GetAll(type);
                        foreach (Entity e in entities)
                        {
                            e.Dispose();
                        }
                    }
                    this.entities.Remove(type);
                }
            }
        }


        public void UnloadComponents(IList<Type> types)
        {
            lock (comlock)
            {
                foreach(Type t in types)
                {
                    var targets = this.components.Where(v => (v.Value).GetType() == t);
                    foreach(KeyValuePair<long,Component> u in targets)
                    {
                        if (u.Value.Entity != null && !u.Value.IsDisposed)
                            u.Value.Entity.RemoveComponent(t);
                        this.components.Remove(u.Key);
                    }
                }
            }
        }

        public void UnloadEntities(IList<Type> types)
        {
            lock (entlock)
            {
                foreach(Type t in types)
                {
                    if (this.entities.ContainsKey(t))
                    {
                        foreach(Entity e in this.entities.GetAll(t))
                        {
                            e.Dispose();
                        }
                        this.entities.Remove(t);
                    }
                }
            }
        }
    

    }
}
