using System;
using System.Reflection;

namespace Konata.Msf
{
    internal static class ServiceRoutine
    {
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
    }
}
