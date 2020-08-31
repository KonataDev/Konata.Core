using System;
using System.Linq;

namespace Konata.Msf
{
    internal static class ServiceRoutine
    {
        internal static Type Get(string name)
        {
            return Type.GetType($"Konata.Msf.Services.{name}");
        }

        internal static bool Run(Core core, string name, params object[] args)
        {
            try
            {
                var type = Get(name);
                var instance = Activator.CreateInstance(type);
                var arguments = new object[] { core }.Concat(args).ToArray();
                return (bool)type.GetMethod("Run").Invoke(instance, args);
            }
            catch (Exception e)
            {
                ((Core)args[0]).EmitError(1, e.StackTrace);
                return false;
            }
        }
    }
}
