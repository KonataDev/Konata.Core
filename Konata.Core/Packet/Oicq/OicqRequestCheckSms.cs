using System;
using Konata.Core.Packet.Tlv;
using Konata.Core.Packet.Tlv.TlvModel;

namespace Konata.Core.Packet.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestCheckSms : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0007;

        public OicqRequestCheckSms(uint uin, string session, string smsToken,
            string smsCode, byte[] gSecret, OicqKeyRing keyRing)

            : base(OicqCommand, OicqSubCommand, uin, OicqEncryptMethod.ECDH7,
                  new XCheckSms(session, smsToken, smsCode, gSecret),
                  keyRing.shareKey, keyRing.randKey, keyRing.defaultPublicKey)
        {

        }

        public class XCheckSms : OicqRequestBody
        {
            public XCheckSms(string session, string smsToken, string smsCode, byte[] gSecret)
            {
                TlvPacker tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body(2052)));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(session)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(Default.WtLoginSdk.MiscBitmap,
                        Default.WtLoginSdk.SubSigBitmap, Default.WtLoginSdk.SubAppIdList)));
                    tlvs.PutTlv(new Tlv(0x0174, new T174Body(smsToken)));
                    tlvs.PutTlv(new Tlv(0x017c, new T17cBody(smsCode)));
                    tlvs.PutTlv(new Tlv(0x0198, new T198Body(0)));
                    tlvs.PutTlv(new Tlv(0x0401, new T401Body(gSecret)));// G
                }

                PutUshortBE(OicqSubCommand);
                PutBytes(tlvs.GetBytes(true));
            }
        }
    }
}
