using System;

namespace Konata.Core.Exceptions.Model
{
    public class OperationFailedException : KonataException
    {
        public OperationFailedException(int code, string message)
            : base(code, message)
        {
        }

        public OperationFailedException(int code, Exception e, string message)
            : base(code, e, message)
        {
        }
    }
}
