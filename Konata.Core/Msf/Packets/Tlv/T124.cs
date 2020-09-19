using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T124 : TlvBase
    {
        private readonly string _osType;
        private readonly string _osVersion;
        private readonly NetworkType _networkType;
        private readonly string _networkDetail;
        private readonly string _address;
        private readonly string _apnName;

        public T124(string osType, string osVersion, NetworkType networkType,
            string networkDetail, string apnName) : base()
        {
            _osType = osType;
            _osVersion = osVersion;
            _networkType = networkType;
            _networkDetail = networkDetail;
            _apnName = apnName;
            _address = "";
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x124);
        }

        public override void PutTlvBody()
        {
            PutString(_osType, 2, 16);
            PutString(_osVersion, 2, 16);
            PutUshortBE((ushort)_networkType);
            PutString(_networkDetail, 2, 16);
            PutString(_address, 2, 32);
            PutString(_apnName, 2, 16);
        }
    }
}
