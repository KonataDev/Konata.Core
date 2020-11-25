using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    public class ServiceManager
    {
        private static ServiceManager instance;

        public static ServiceManager Instance
        {
            get => instance ?? (instance = new ServiceManager());
        }

        private ServiceManager()
        {

        }

        private readonly Dictionary<Type, IDisposable> services = new Dictionary<Type, IDisposable>();

        private object serlock = new object();

        public bool AddNewService<T>()
        {
            lock (serlock)
            {
                Type type = typeof(T);
                if (this.services.ContainsKey(type))
                {
                    return false;
                }
                T obj = (T)Activator.CreateInstance(type);
                IDisposable service = obj as IDisposable;
                if (service == null)
                {
                    return false;
                }
                this.services.Add(type, service);
                if (typeof(ILoad).IsAssignableFrom(type))
                {
                    (obj as ILoad).Load();
                }
                return true;
            }
        }

        public bool AddNewService(Type type)
        {
            lock (serlock)
            {
                if (this.services.ContainsKey(type))
                {
                    return false;
                }
                Object obj = Activator.CreateInstance(type);
                IDisposable service = obj as IDisposable;
                if (service == null)
                {
                    return false;
                }
                this.services.Add(type, service);
                if (typeof(ILoad).IsAssignableFrom(type))
                {
                    (obj as ILoad).Load();
                }
                return true;
            }
        }

        public void RemoveService(Type type)
        {
            lock (serlock)
            {
                if (this.services.TryGetValue(type,out IDisposable service))
                {
                    service.Dispose();
                    this.services.Remove(type);
                }
            }
        }

        public void RemoveService<T>()
        {
            Type type = typeof(T);
            lock (serlock)
            {
                if (this.services.TryGetValue(type, out IDisposable service))
                {
                    service.Dispose();
                    this.services.Remove(type);
                }
            }
        }

        public T GetService<T>()
            where T:IDisposable
        {
            Type type = typeof(T);
            if(!this.services.TryGetValue(type,out IDisposable service))
            {
                return default(T);
            }
            return (T)service;
        }

        public void LoadServices(IList<Type> types)
        {
            foreach (Type t in types)
            {
                bool resault=this.AddNewService(t);
                //unable load service t.Name
            }
        }

        public void UnloadServices(IList<Type> types)
        {
            foreach (Type t in types)
            {
                this.RemoveService(t);
            }
        }
    }
}
