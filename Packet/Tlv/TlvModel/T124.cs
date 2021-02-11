using System;

using Konata.Core;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T124Body : TlvBody
    {
        private readonly string _osType;
        private readonly string _osVersion;
        private readonly NetworkType _networkType;
        private readonly string _networkDetail;
        private readonly string _address;
        private readonly string _apnName;

        public T124Body(string osType, string osVersion, NetworkType networkType,
            string networkDetail, string apnName)
            : base()
        {
            _osType = osType;
            _osVersion = osVersion;
            _networkType = networkType;
            _networkDetail = networkDetail;
            _apnName = apnName;
            _address = "";

            PutString(_osType, Prefix.Uint16, 16);
            PutString(_osVersion, Prefix.Uint16, 16);
            PutUshortBE((ushort)_networkType);
            PutString(_networkDetail, Prefix.Uint16, 16);
            PutString(_address, Prefix.Uint16, 32);
            PutString(_apnName, Prefix.Uint16, 16);
        }

        public T124Body(byte[] data)
            : base(data)
        {
            TakeString(out _osType, Prefix.Uint16);
            TakeString(out _osVersion, Prefix.Uint16);
            TakeUshortBE(out var type); _networkType = (NetworkType)type;
            TakeString(out _networkDetail, Prefix.Uint16);
            TakeString(out _address, Prefix.Uint16);
            TakeString(out _apnName, Prefix.Uint16);
        }
    }
}
