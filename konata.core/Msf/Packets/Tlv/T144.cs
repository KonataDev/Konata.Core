using Konata.Utils.Crypt;
using Konata.Protocol.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T144 : TlvBase
    {
        private readonly T109 _tlv109;
        private readonly T52d _tlv52d;
        private readonly T124 _tlv124;
        private readonly T128 _tlv128;
        private readonly T148 _tlv148;
        private readonly T153 _tlv153;
        private readonly T16e _tlv16e;
        private readonly byte[] _tgtgKey;

        public T144(T109 tlv109, T52d tlv52d, T124 tlv124, T128 tlv128,
            T148 tlv148, T153 tlv153, T16e tlv16e, byte[] tgtgKey)
        {
            _tlv109 = tlv109;
            _tlv52d = tlv52d;
            _tlv124 = tlv124;
            _tlv128 = tlv128;
            _tlv148 = tlv148;
            _tlv153 = tlv153;
            _tlv16e = tlv16e;
            _tgtgKey = tgtgKey;
        }

        public T144(T109 tlv109, T52d tlv52d, T124 tlv124, T128 tlv128, T16e tlv16e, byte[] tgtgKey)
        {
            _tlv109 = tlv109;
            _tlv52d = tlv52d;
            _tlv124 = tlv124;
            _tlv128 = tlv128;
            _tlv16e = tlv16e;
            _tgtgKey = tgtgKey;
        }

        public T144(string androidId, byte[] deviceDevInfo, string osType, string osVersion,
            NetworkType networkType, string networkDetail, string apnName,
            bool isNewInstall, bool isGuidAvaliable, bool isGuidChanged, byte[] guid, int guidFlag,
            string deviceModel, string deviceBrand, byte[] tgtgKey)
        {
            _tlv109 = new T109(androidId);
            _tlv16e = new T16e(deviceModel);
            _tlv52d = new T52d(deviceDevInfo);
            _tlv124 = new T124(osType, osVersion, networkType, networkDetail, apnName);
            _tlv128 = new T128(isNewInstall, isGuidAvaliable, isGuidChanged, guid, guidFlag, deviceModel, deviceBrand);
            _tgtgKey = tgtgKey;
        }

        public override ushort GetTlvCmd()
        {
            return 0x144;
        }

        public override byte[] GetTlvBody()
        {
            TlvPacker packer = new TlvPacker();
            packer.PushTlv(_tlv109);
            packer.PushTlv(_tlv52d);
            packer.PushTlv(_tlv124);
            packer.PushTlv(_tlv128);
            packer.PushTlv(_tlv148);
            packer.PushTlv(_tlv153);
            packer.PushTlv(_tlv16e);
            return packer.GetEncryptedPacket(true, new TeaCryptor(), _tgtgKey);
        }
    }
}
