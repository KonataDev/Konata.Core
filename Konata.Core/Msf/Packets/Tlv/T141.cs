using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T141 : TlvBase
    {
        public T141(string simOperatorName, NetworkType networkType, string apnName)
            : base(0x0141, new T141Body(simOperatorName, networkType, apnName))
        {

        }
    }

    public class T141Body : TlvBody
    {
        public readonly ushort _version;
        public readonly string _apnName;
        public readonly string _simOperatorName;
        public readonly NetworkType _networkType;

        public T141Body(string simOperatorName, NetworkType networkType, string apnName)
            : base()
        {
            _version = 1;
            _simOperatorName = simOperatorName;
            _networkType = networkType;
            _apnName = apnName;

            PutUshortBE(_version);
            PutString(_simOperatorName, 2);
            PutUshortBE((ushort)_networkType);
            PutString(_apnName, 2);
        }

        public T141Body(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _version);
            TakeString(out _simOperatorName, Prefix.Uint16);
            TakeUshortBE(out var type); _networkType = (NetworkType)type;
            TakeString(out _apnName, Prefix.Uint16);
        }
    }
}
