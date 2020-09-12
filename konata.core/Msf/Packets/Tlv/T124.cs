using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
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
            string networkDetail, string apnName)
        {
            _osType = osType;
            _osVersion = osVersion;
            _networkType = networkType;
            _networkDetail = networkDetail;
            _apnName = apnName;
            _address = "";
        }

        public override ushort GetTlvCmd()
        {
            return 0x124;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutString(_osType, true, true, 16);
            builder.PutString(_osVersion, true, true, 16);
            builder.PutUshortBEBE((short)_networkType);
            builder.PutString(_networkDetail, true, true, 16);
            builder.PutString(_address, true, true, 32);
            builder.PutString(_apnName, true, true, 16);
            return builder.GetBytes();
        }
    }
}
