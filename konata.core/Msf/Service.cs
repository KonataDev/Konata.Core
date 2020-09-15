using System;

namespace Konata.Msf
{
    internal abstract class Service
    {
        internal string name;

        internal abstract bool Run(Core core, string method, params object[] args);

        internal abstract bool Handle(Core core, params object[] args);
    }
}
