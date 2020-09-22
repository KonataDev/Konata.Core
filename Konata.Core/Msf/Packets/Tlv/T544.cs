using System;

namespace Konata.Msf.Packets.Tlv
{
    /// <summary>
    /// 未完成
    /// </summary>
    public class T544 : TlvBase
    {
        public T544(string wtLoginSdk)
            : base(0x0544, new T544Body(wtLoginSdk))
        {

        }
    }

    public class T544Body : TlvBody
    {
        public readonly string _wtLoginSdk;

        public T544Body(string wtLoginSdk)
            : base()
        {
            _wtLoginSdk = wtLoginSdk;

        }

        public T544Body(byte[] data)
            : base(data)
        {
            EatBytes(RemainLength);
        }
    }
}
