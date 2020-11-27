using System;
using Konata.Model.Packet.Tlv;
using Konata.Model.Packet.Tlv.TlvModel;

namespace Konata.Model.Packet.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestCheckSms : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0007;

        public OicqRequestCheckSms(SigInfo sigInfo, string smsCode)

            : base(OicqCommand, OicqSubCommand, sigInfo.Uin, OicqEncryptMethod.ECDH7,
                  new XCheckSms(sigInfo.WtLoginSession, sigInfo.GSecret, sigInfo.WtLoginSmsToken, smsCode),
                  sigInfo.ShareKey, sigInfo.RandKey, sigInfo.DefaultPublicKey)
        {

        }

        public class XCheckSms : OicqRequestBody
        {
            public XCheckSms(string sigSession, byte[] gSecret, string smsToken, string smsCode)
            {
                TlvPacker tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body(2052)));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(sigSession)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(AppInfo.wtLoginMiscBitmap,
                        AppInfo.wtLoginSubSigBitmap, AppInfo.wtLoginSubAppIdList)));
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
