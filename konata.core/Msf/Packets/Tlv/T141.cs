using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T141 : TlvBase
    {
        private const short _version = 1;

        private readonly string _apnName;
        private readonly string _simOperatorName;
        private readonly NetworkType _networkType;

        public T141(string simOperatorName, NetworkType networkType, string apnName)
        {
            _simOperatorName = simOperatorName;
            _networkType = networkType;
            _apnName = apnName;
        }

        public override ushort GetTlvCmd()
        {
            return 0x141;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt16(_version);
            builder.PushString(_simOperatorName);
            builder.PushInt16((short)_networkType);
            builder.PushString(_apnName);
            return builder.GetBytes();
        }
    }
}
