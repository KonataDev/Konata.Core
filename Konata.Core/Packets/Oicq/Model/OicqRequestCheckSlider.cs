using System;
using Konata.Core.Common;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.Model;

namespace Konata.Core.Packets.Oicq.Model;

using Tlv = Tlv.Tlv;

internal class OicqRequestCheckSlider : OicqRequest
{
    private const ushort OicqCommand = 0x0810;
    private const ushort OicqSubCommand = 0x0002;

    public OicqRequestCheckSlider(string ticket, AppInfo appInfo, BotKeyStore signinfo)
        : base(OicqCommand, signinfo.Account.Uin, signinfo.Ecdh.MethodId,
            signinfo.KeyStub.RandKey, signinfo.Ecdh, appInfo, w =>
            {
                var tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0193, new T193Body(ticket)));
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body()));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(signinfo.Session.WtLoginSession)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(appInfo.WtLoginSdk.MiscBitmap,
                        appInfo.WtLoginSdk.SubSigBitmap, appInfo.WtLoginSdk.SubAppIdList)));

                    if (signinfo.Session.WtSessionT547 != null)
                        tlvs.PutTlv(new Tlv(0x0547, signinfo.Session.WtSessionT547));
                }

                w.PutUshortBE(OicqSubCommand);
                w.PutBytes(tlvs.GetBytes(true));
            })
    {
    }
}
