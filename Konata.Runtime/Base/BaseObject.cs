using System;
using Konata.Runtime.Utils;

namespace Konata.Runtime.Base
{
    public abstract class BaseObject : IDisposable
    {
        public long Id { get; set; }

        public virtual bool Recycle { get; set; } = true;

        public bool IsDisposed
        {
            get => (Id == 0);
        }

        protected BaseObject()
            => Id = IdGenerater.GenerateID();

        protected BaseObject(long id)
            => Id = id;

        ~BaseObject()
        {
            Id = 0;
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                Id = 0;
                if (Recycle)
                    ObjectPool.Instance.Recycle(this);
            }
        }
    }
}
