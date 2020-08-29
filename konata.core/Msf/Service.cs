using System;

namespace Konata.Msf
{
    internal abstract class Service
    {
        internal static bool Run(Core core) => false;

        internal static bool Handle(byte[] data, Core core) => false;
    }
}
