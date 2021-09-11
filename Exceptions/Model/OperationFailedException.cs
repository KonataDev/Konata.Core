using System;

namespace Konata.Core.Exceptions.Model
{
    public class OperationFailedException : KonataException
    {
        public OperationFailedException(int code, string message)
            : base(code, message)
        {
        }
    }
}
