using System;

namespace Konata.Core.Exceptions.Model
{
    public class CoreOfflineException : KonataException
    {
        public CoreOfflineException(int code, string message)
            : base(code, message)
        {
        }
    }
}
