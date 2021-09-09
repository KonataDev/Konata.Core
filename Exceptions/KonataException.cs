using System;

namespace Konata.Core.Exceptions
{
    public abstract class KonataException : Exception
    {
        protected KonataException(int code, string message)
            : base(message)
        {
            HResult = code;
        }

        protected KonataException(int code, Exception e, string message)
            : base(message, e)
        {
            HResult = code;
        }
    }
}
