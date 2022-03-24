using System;

namespace Konata.Core.Exceptions;

public abstract class KonataException : Exception
{
    protected KonataException(string message)
        : base(message)
    {
    }

    protected KonataException(string message, Exception e)
        : base(message, e)
    {
    }

    protected KonataException(int code, string message)
        : base(message)
    {
        HResult = code;
    }
}
