using System;

namespace Konata.Core.Packet.Tlv.TlvModel
{
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
            PutString(_simOperatorName, Prefix.Uint16);
            PutUshortBE((ushort)_networkType);
            PutString(_apnName, Prefix.Uint16);
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
