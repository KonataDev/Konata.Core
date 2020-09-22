using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T144 : TlvBase
    {
        public T144(T109 tlv109, T52d tlv52d, T124 tlv124, T128 tlv128,
            T148 tlv148, T153 tlv153, T16e tlv16e, byte[] tgtgKey)
            : base(0x0144, new T144Body(tlv109, tlv52d, tlv124, tlv128,
             tlv148, tlv153, tlv16e), tgtgKey)
        {

        }

        public T144(T109 tlv109, T52d tlv52d, T124 tlv124, T128 tlv128,
            T16e tlv16e, byte[] tgtgKey)
            : base(0x0144, new T144Body(tlv109, tlv52d, tlv124, tlv128,
             tlv16e), tgtgKey)
        {

        }

        public T144(string androidId, byte[] deviceDevInfo, string osType, string osVersion,
            NetworkType networkType, string networkDetail, string apnName, bool isNewInstall,
            bool isGuidAvaliable, bool isGuidChanged, byte[] guid, uint guidFlag,
            string deviceModel, string deviceBrand, byte[] tgtgKey)

            : base(0x0144, new T144Body(androidId, deviceDevInfo, osType, osVersion,
             networkType, networkDetail, apnName, isNewInstall,
             isGuidAvaliable, isGuidChanged, guid, guidFlag,
             deviceModel, deviceBrand), tgtgKey)
        {

        }
    }

    public class T144Body : TlvBody
    {
        public readonly T109 _tlv109;
        public readonly T52d _tlv52d;
        public readonly T124 _tlv124;
        public readonly T128 _tlv128;
        public readonly T148 _tlv148;
        public readonly T153 _tlv153;
        public readonly T16e _tlv16e;

        public T144Body(T109 tlv109, T52d tlv52d, T124 tlv124, T128 tlv128,
            T148 tlv148, T153 tlv153, T16e tlv16e)
            : base()
        {
            _tlv109 = tlv109;
            _tlv52d = tlv52d;
            _tlv124 = tlv124;
            _tlv128 = tlv128;
            _tlv148 = tlv148;
            _tlv153 = tlv153;
            _tlv16e = tlv16e;

            PutT144Body();
        }

        public T144Body(T109 tlv109, T52d tlv52d, T124 tlv124, T128 tlv128,
            T16e tlv16e)
        {
            _tlv109 = tlv109;
            _tlv52d = tlv52d;
            _tlv124 = tlv124;
            _tlv128 = tlv128;
            _tlv16e = tlv16e;

            PutT144Body();
        }

        public T144Body(string androidId, byte[] deviceDevInfo, string osType, string osVersion,
            NetworkType networkType, string networkDetail, string apnName, bool isNewInstall,
            bool isGuidAvaliable, bool isGuidChanged, byte[] guid, uint guidFlag,
            string deviceModel, string deviceBrand)
        {
            _tlv109 = new T109(androidId);
            _tlv16e = new T16e(deviceModel);
            _tlv52d = new T52d(deviceDevInfo);
            _tlv124 = new T124(osType, osVersion, networkType, networkDetail, apnName);
            _tlv128 = new T128(isNewInstall, isGuidAvaliable, isGuidChanged, guid, guidFlag, deviceModel, deviceBrand);

            PutT144Body();
        }

        private void PutT144Body()
        {
            TlvPacker packer = new TlvPacker();
            packer.PutTlv(_tlv109);
            packer.PutTlv(_tlv52d);
            packer.PutTlv(_tlv124);
            packer.PutTlv(_tlv128);
            packer.PutTlv(_tlv148);
            packer.PutTlv(_tlv153);
            packer.PutTlv(_tlv16e);
            PutBytes(packer.GetBytes(true));
        }

        public T144Body(byte[] data)
            : base(data)
        {
             
        }
    }
}
