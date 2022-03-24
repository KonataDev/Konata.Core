namespace Konata.Core.Exceptions.Model;

public class InvalidFaceIdException : KonataException
{
    public InvalidFaceIdException(string message)
        : base(message)
    {
    }
}
