using System;
using System.Threading.Tasks;

namespace Konata.Msf.Interfaces
{
    internal interface IService
    {
        bool Run(Core core);

        bool Handle(byte[] data, Core core);
    }
}
