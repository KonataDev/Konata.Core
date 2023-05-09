using Konata.Core.Common;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.Model;

namespace Konata.Core.Packets.Oicq.Model;

using Tlv = Tlv.Tlv;

internal class OicqRequestCheckSms : OicqRequest
{
    private const ushort OicqCommand = 0x0810;
    private const ushort OicqSubCommand = 0x0007;

    public OicqRequestCheckSms(string code, AppInfo appInfo, BotKeyStore signinfo, BotDevice device)
        : base(OicqCommand, signinfo.Account.Uin, signinfo.Ecdh.MethodId,
            signinfo.KeyStub.RandKey, signinfo.Ecdh, appInfo, w =>
            {
                var tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body(2052)));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(signinfo.Session.WtLoginSession)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(appInfo.WtLoginSdk.MiscBitmap,
                        appInfo.WtLoginSdk.SubSigBitmap, appInfo.WtLoginSdk.SubAppIdList)));
                    tlvs.PutTlv(new Tlv(0x0174, new T174Body(signinfo.Session.WtLoginSmsToken)));
                    tlvs.PutTlv(new Tlv(0x017c, new T17cBody(code)));
                    tlvs.PutTlv(new Tlv(0x0198, new T198Body(0)));
                    tlvs.PutTlv(new Tlv(0x0401, new T401Body(signinfo.Session.GSecret))); // G
                    tlvs.PutTlv(new Tlv(0x0544, new T544Body(OicqSubCommand, 2, 
                        appInfo.WtLoginSdk.SdkVersion, device.System.Guid, signinfo.Account.Uin)));                }

                w.PutUshortBE(OicqSubCommand);
                w.PutBytes(tlvs.GetBytes(true));
            })
    {
    }
}
