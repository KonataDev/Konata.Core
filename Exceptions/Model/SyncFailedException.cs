using System;

namespace Konata.Core.Exceptions.Model
{
    public class SyncFailedException : KonataException
    {
        public SyncFailedException(int code, string message)
            : base(code, message)
        {
        }

        public SyncFailedException(int code, Exception e, string message)
            : base(code, e, message)
        {
        }
    }
}
