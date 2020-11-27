using System;
using Konata.Runtime.Utils;

namespace Konata.Runtime.Base
{
    public abstract class BaseObject : IDisposable
    {
        public long Id { get; set; }

        public bool Recycle { get; set; } = true;

        protected BaseObject()
        {
            this.Id = IdGenerater.GenerateID();
        }

        protected BaseObject(long id)
        {
            this.Id = id;
        }

        public bool IsDisposed
        {
            get => (this.Id == 0);
        }

        public virtual void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.Id = 0;
                if (Recycle)
                    ObjectPool.Instance.Recycle(this);
            }
        }

        ~BaseObject()
        {
            this.Id = 0;
        }
    }
}
