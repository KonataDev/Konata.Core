// #define KONATA_USE_REFLECTION

using System;
using System.Collections.Generic;

namespace Konata.Msf
{
    using Routine = Dictionary<string, Service>;

    internal static class ServiceRoutine
    {

#if KONATA_USE_REFLECTION
        internal static Type Get(string name)
        {
            return Type.GetType($"Konata.Msf.Services.{name}", true, true);
        }

        internal static bool Run(Core core, string name, string method, params object[] args)
        {
            try
            {
                var type = Get(name);
                var instance = (Service)type
                    .GetProperty("Instance",
                        BindingFlags.SetProperty |
                        BindingFlags.Public |
                        BindingFlags.Static)
                    .GetValue(null);

                return instance.Run(core, method, args);
            }
            catch (Exception e)
            {
                ((Core)args[0]).EmitError(1, e.StackTrace);
                return false;
            }
        }
#else
        internal static readonly Routine Map = new Routine
        {
            { "wtlogin.login", Services.Wtlogin.Login.Instance }
        };

        internal static bool Run(Core core, string name, string method, params object[] args)
        {
            try
            {
                return Map[name.ToLower()].Run(core, method, args);
            }
            catch (Exception e)
            {
                ((Core)args[0]).EmitError(1, e.StackTrace);
                return false;
            }
        }
#endif

    }
}
