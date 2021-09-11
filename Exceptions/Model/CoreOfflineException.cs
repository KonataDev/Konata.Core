using System;

namespace Konata.Core.Exceptions.Model
{
    public class CoreOfflineException : KonataException
    {
        public CoreOfflineException(int code, string message)
            : base(code, message)
        {
        }

        public CoreOfflineException(int code, Exception e, string message)
            : base(code, e, message)
        {
        }
    }
}
