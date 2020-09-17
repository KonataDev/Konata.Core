using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
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

        public override void PutTlvCmd()
        {
            PutUshortBE(0x141);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(_version);
            PutString(_simOperatorName, 2);
            PutUshortBE((ushort)_networkType);
            PutString(_apnName, 2);
        }
    }
}
