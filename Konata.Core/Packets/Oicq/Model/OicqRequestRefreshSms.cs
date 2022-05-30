using Konata.Core.Common;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.Model;

namespace Konata.Core.Packets.Oicq.Model;

using Tlv = Tlv.Tlv;

internal class OicqRequestRefreshSms : OicqRequest
{
    private const ushort OicqCommand = 0x0810;
    private const ushort OicqSubCommand = 0x0008;

    public OicqRequestRefreshSms(AppInfo appInfo, BotKeyStore keystore)
        : base(OicqCommand, keystore.Account.Uin, keystore.Ecdh.MethodId,
            keystore.KeyStub.RandKey, keystore.Ecdh, appInfo, w =>
            {
                var tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body(2052)));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(keystore.Session.WtLoginSession)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(appInfo.WtLoginSdk.MiscBitmap,
                        appInfo.WtLoginSdk.SubSigBitmap, appInfo.WtLoginSdk.SubAppIdList)));
                    tlvs.PutTlv(new Tlv(0x0174, new T174Body(keystore.Session.WtLoginSmsToken)));
                    tlvs.PutTlv(new Tlv(0x017a, new T17aBody(9)));
                    tlvs.PutTlv(new Tlv(0x0197, new T197Body(0)));
                }

                w.PutUshortBE(OicqSubCommand);
                w.PutBytes(tlvs.GetBytes(true));
            })
    {
    }
}
