using Konata.Core.Common;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.Model;

namespace Konata.Core.Packets.Oicq.Model
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestCheckSms : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0007;

        public OicqRequestCheckSms(string code, BotKeyStore signinfo)
            : base(OicqCommand, signinfo.Account.Uin, OicqEncryptMethod.ECDH7,
                signinfo.KeyStub.ShareKey, signinfo.KeyStub.RandKey,
                signinfo.KeyStub.DefaultPublicKey, w =>
                {
                    TlvPacker tlvs = new TlvPacker();
                    {
                        tlvs.PutTlv(new Tlv(0x0008, new T8Body(2052)));
                        tlvs.PutTlv(new Tlv(0x0104, new T104Body(signinfo.Session.WtLoginSession)));
                        tlvs.PutTlv(new Tlv(0x0116, new T116Body(AppInfo.WtLoginSdk.MiscBitmap,
                            AppInfo.WtLoginSdk.SubSigBitmap, AppInfo.WtLoginSdk.SubAppIdList)));
                        tlvs.PutTlv(new Tlv(0x0174, new T174Body(signinfo.Session.WtLoginSmsToken)));
                        tlvs.PutTlv(new Tlv(0x017c, new T17cBody(code)));
                        tlvs.PutTlv(new Tlv(0x0198, new T198Body(0)));
                        tlvs.PutTlv(new Tlv(0x0401, new T401Body(signinfo.Session.GSecret))); // G
                    }

                    w.PutUshortBE(OicqSubCommand);
                    w.PutBytes(tlvs.GetBytes(true));
                })
        {
        }
    }
}
