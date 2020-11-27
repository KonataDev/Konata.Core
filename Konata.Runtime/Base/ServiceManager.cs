using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Konata.Runtime.Base
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

        private readonly Dictionary<Type, IDisposable> services
            = new Dictionary<Type, IDisposable>();

        private ReaderWriterLockSlim servicesLock
            = new ReaderWriterLockSlim();

        public bool AddNewService<T>()
        {
            Type type = typeof(T);
            servicesLock.EnterWriteLock();
            try
            {
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
            finally
            {
                servicesLock.ExitWriteLock();
            }
        }

        public bool AddNewService(Type type)
        {
            servicesLock.EnterWriteLock();
            try
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
            finally
            {
                servicesLock.ExitWriteLock();
            }
        }

        public void RemoveService(Type type)
        {
            servicesLock.EnterWriteLock();
            try
            {
                if (this.services.TryGetValue(type, out IDisposable service))
                {
                    service.Dispose();
                    this.services.Remove(type);
                }
            }
            finally
            {
                servicesLock.ExitWriteLock();
            }
        }

        public void RemoveService<T>()
        {
            Type type = typeof(T);
            servicesLock.EnterWriteLock();
            try
            {
                if (this.services.TryGetValue(type, out IDisposable service))
                {
                    service.Dispose();
                    this.services.Remove(type);
                }
            }
            finally
            {
                servicesLock.ExitWriteLock();
            }
        }

        public T GetService<T>()
            where T : IDisposable
        {
            servicesLock.EnterReadLock();
            try
            {
                Type type = typeof(T);
                if (!this.services.TryGetValue(type, out IDisposable service))
                {
                    return default(T);
                }
                return (T)service;
            }
            finally
            {
                servicesLock.ExitReadLock();
            }

        }

        public void LoadServices(IList<Type> types)
        {
            foreach (Type t in types)
            {
                bool resault = this.AddNewService(t);
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
