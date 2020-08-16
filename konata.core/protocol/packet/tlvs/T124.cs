using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
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
            builder.PushString(_osType, true, true, 16);
            builder.PushString(_osVersion, true, true, 16);
            builder.PushInt16((short)_networkType);
            builder.PushString(_networkDetail, true, true, 16);
            builder.PushString(_address, true, true, 32);
            builder.PushString(_apnName, true, true, 16);
            return builder.GetBytes();
        }
    }
}
