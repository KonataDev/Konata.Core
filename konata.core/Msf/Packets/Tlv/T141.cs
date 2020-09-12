using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T141 : TlvBase
    {
        private const ushort _version = 1;

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
            builder.PutUshortBE(_version);
            builder.PutString(_simOperatorName, 2);
            builder.PutUshortBE((ushort)_networkType);
            builder.PutString(_apnName);
            return builder.GetBytes();
        }
    }
}
