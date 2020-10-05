using System;
using Konata.Msf.Crypto;
using Konata.Msf.Packets.Tlv;

namespace Konata.Msf.Packets.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestRefreshSms : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0008;

        public OicqRequestRefreshSms(uint uin, KeyRing keyring,
            string sigSission, byte[] sigSecret)

            : base(OicqCommand, OicqSubCommand, uin,
                  OicqEncryptMethod.ECDH7, new XRefreshSms(sigSission, sigSecret),
                  keyring._shareKey, keyring._randKey, keyring._defaultPublicKey)
        {

        }

        public class XRefreshSms : OicqRequestBody
        {
            public XRefreshSms(string sigSission, byte[] sigSecret)
                : base()
            {
                TlvPacker tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body(2052)));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(sigSission)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(AppInfo.wtLoginMiscBitmap,
                        AppInfo.wtLoginSubSigBitmap, AppInfo.wtLoginSubAppIdList)));
                    tlvs.PutTlv(new Tlv(0x0174, new T174Body(sigSecret, null)));
                    tlvs.PutTlv(new Tlv(0x017a, new T17aBody(9)));
                    tlvs.PutTlv(new Tlv(0x0197, new T197Body(0)));
                }

                PutUshortBE(OicqSubCommand);
                PutBytes(tlvs.GetBytes(true));
            }
        }
    }
}
