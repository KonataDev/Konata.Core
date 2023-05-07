using System;
using System.IO;
using System.Text;
using Konata.Core.Utils.TencentEncrypt;

namespace Konata.Core.Packets.Tlv.Model;

internal class T544Body : TlvBody
{
    public T544Body(uint subCmd, int v, string sdkVersion, byte[] guid, uint userId)
        : base()
    {
        var salt = new TlvBody();
        if (v == 2) // Tlv544v2
        {
            salt.PutUintLE(0);
            salt.PutBytes(guid);
            salt.PutBytes(Encoding.ASCII.GetBytes(sdkVersion));
            salt.PutUintLE(subCmd);
            salt.PutUintLE(0);
        }
        else
        {
            salt.PutUintLE(userId);
            salt.PutBytes(guid);
            salt.PutBytes(Encoding.ASCII.GetBytes(sdkVersion));
            salt.PutUintLE(subCmd);
        }
        var sign = Algorithm.Sign((uint)(DateTime.UtcNow.Ticks / 10), salt.GetBytes());
        PutBytes(sign);
    }

    public T544Body(byte[] data)
        : base(data)
    {
        EatBytes(RemainLength);
    }
}
