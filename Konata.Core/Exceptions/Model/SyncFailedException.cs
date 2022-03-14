namespace Konata.Core.Exceptions.Model;

public class SyncFailedException : KonataException
{
    public SyncFailedException(int code, string message)
        : base(code, message)
    {
    }
}
