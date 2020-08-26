using System;
using System.Threading.Tasks;

namespace Konata.Msf
{
    internal static class ServiceRoutine
    {
        internal static Type Get(string name)
        {
            return Type.GetType($"Konata.Msf.Services.{name}");
        }

        internal static bool Run(string name, Core core)
        {
            try
            {
                var type = Get(name);
                var instance = Activator.CreateInstance(type);
                return (bool)type.GetMethod("Run").Invoke(instance, new object[] { core });
            }
            catch (Exception e)
            {
                core.EmitError(1, e.StackTrace);
                return false;
            }
        }
    }
}
