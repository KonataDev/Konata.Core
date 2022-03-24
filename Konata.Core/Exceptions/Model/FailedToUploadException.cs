using System;

namespace Konata.Core.Exceptions.Model;

public class FailedToUploadException : KonataException
{
    public FailedToUploadException(Exception e)
        : base("Upload resources failed.", e)
    {
    }
}
