using System;

using Konata.Core.Packet.Protobuf;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T144Body : TlvBody
    {
        public readonly Tlv _tlv109;
        public readonly Tlv _tlv52d;
        public readonly Tlv _tlv124;
        public readonly Tlv _tlv128;
        public readonly Tlv _tlv148;
        public readonly Tlv _tlv153;
        public readonly Tlv _tlv16e;

        public T144Body(Tlv tlv109, Tlv tlv52d, Tlv tlv124, Tlv tlv128,
            Tlv tlv148, Tlv tlv153, Tlv tlv16e)
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

        public T144Body(Tlv tlv109, Tlv tlv52d, Tlv tlv124, Tlv tlv128,
            Tlv tlv16e)
        {
            _tlv109 = tlv109;
            _tlv52d = tlv52d;
            _tlv124 = tlv124;
            _tlv128 = tlv128;
            _tlv16e = tlv16e;

            PutT144Body();
        }

        public T144Body(string androidId, DeviceReport deviceReport, string osType, string osVersion,
            NetworkType networkType, string networkDetail, string apnName, bool isNewInstall,
            bool isGuidAvaliable, bool isGuidChanged, byte[] guid, uint guidFlag,
            string deviceModel, string deviceBrand)
        {
            _tlv109 = new Tlv(0x0109, new T109Body(androidId));
            _tlv16e = new Tlv(0x016e, new T16eBody(deviceModel));
            _tlv52d = new Tlv(0x052d, new T52dBody(deviceReport));
            _tlv124 = new Tlv(0x0124, new T124Body(osType, osVersion, networkType, networkDetail, apnName));
            _tlv128 = new Tlv(0x0128, new T128Body(isNewInstall, isGuidAvaliable, isGuidChanged, guid, guidFlag, deviceModel, deviceBrand));

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
            while (RemainLength > 0)
            {
                TakeTlvData(out byte[] body, out ushort cmd);
                Tlv tlv = new Tlv(cmd, body);
                switch (cmd)
                {
                case 0x0109: _tlv109 = tlv; break;
                case 0x052d: _tlv52d = tlv; break;
                case 0x0124: _tlv124 = tlv; break;
                case 0x0128: _tlv128 = tlv; break;
                case 0x0148: _tlv148 = tlv; break;
                case 0x0153: _tlv153 = tlv; break;
                case 0x016e: _tlv16e = tlv; break;
                default: break; // Unknown Tlv.
                }
            }
        }
    }
}
