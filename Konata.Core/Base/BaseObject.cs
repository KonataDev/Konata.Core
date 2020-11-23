using Konata.Core.Utils;
using System;

namespace Konata.Core.Base
{
    public abstract class BaseObject : IDisposable
    {
        public long Id { get; set; }

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
                ObjectPool.Instance.Recycle(this);
            }
        }

        ~BaseObject()
        {
            this.Id = 0;
        }
    }
}
