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

        internal static bool Run(Core core, string name, string method, params object[] args)
        {
            try
            {
                var type = Get(name);
                //var instance = Activator.CreateInstance(type);
                var instance = (Service)type.GetProperty("Instance").GetValue(null);
                //var arguments = new object[] { core, method }.Concat(args).ToArray();
                //return (bool)type.GetMethod("Run").Invoke(instance, args);
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