using System;

namespace Konata.Utils
{
    public static class Guid
    {
        public static byte[] Generate()
        {
            return new System.Guid().ToByteArray();
        }
    }
}
