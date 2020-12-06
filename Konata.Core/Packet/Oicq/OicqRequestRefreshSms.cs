using System;
using Konata.Core.Packet.Tlv;
using Konata.Core.Packet.Tlv.TlvModel;

namespace Konata.Core.Packet.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestRefreshSms : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0008;

        public OicqRequestRefreshSms(uint uin, string session, string smsToken, OicqKeyRing keyRing)
            : base(OicqCommand, OicqSubCommand, uin, OicqEncryptMethod.ECDH7,
                  new XRefreshSms(session, smsToken),
                  keyRing.shareKey, keyRing.randKey, keyRing.defaultPublicKey)
        {

        }

        public class XRefreshSms : OicqRequestBody
        {
            public XRefreshSms(string session, string smsToken)
                : base()
            {
                TlvPacker tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body(2052)));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(session)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(Default.WtLoginSdk.MiscBitmap,
                        Default.WtLoginSdk.SubSigBitmap, Default.WtLoginSdk.SubAppIdList)));
                    tlvs.PutTlv(new Tlv(0x0174, new T174Body(smsToken)));
                    tlvs.PutTlv(new Tlv(0x017a, new T17aBody(9)));
                    tlvs.PutTlv(new Tlv(0x0197, new T197Body(0)));
                }

                PutUshortBE(OicqSubCommand);
                PutBytes(tlvs.GetBytes(true));
            }
        }
    }
}
