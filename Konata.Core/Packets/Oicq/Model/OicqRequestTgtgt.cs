using Konata.Core.Common;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.Model;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Crypto;

namespace Konata.Core.Packets.Oicq.Model;

using Tlv = Tlv.Tlv;

internal class OicqRequestTgtgt : OicqRequest
{
    private const ushort OicqCommand = 0x0810;
    private const ushort OicqSubCommand = 0x0009;

    public OicqRequestTgtgt(int sequence, AppInfo appInfo, BotKeyStore signinfo, BotDevice device)
        : base(OicqCommand, signinfo.Account.Uin, EcdhCryptor.CryptId.ECDH135,
            signinfo.KeyStub.RandKey, signinfo.Ecdh, appInfo, w =>
            {
                // Device info report
                var report = new DeviceReport(
                    device.System.BootLoader,
                    device.System.Version,
                    device.Model.CodeName,
                    device.System.Incremental,
                    device.System.FingerPrint,
                    device.System.BootId,
                    device.System.AndroidId,
                    device.Model.BaseBand,
                    device.System.InnerVersion);

                // Pack tlv
                var tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0018, new T18Body(appInfo.AppId,
                        appInfo.AppClientVersion, signinfo.Account.Uin)));
                    tlvs.PutTlv(new Tlv(0x0001, new T1Body(signinfo.Account.Uin,
                        device.Network.NetIpAddress)));

                    tlvs.PutTlv(new Tlv(0x0106, new T106Body(appInfo.AppId,
                        appInfo.SubAppId, appInfo.AppClientVersion,
                        signinfo.Account.Uin, new byte[4], true, signinfo.Account.PasswordMd5,
                        0, true, device.System.Guid, LoginType.Password,
                        signinfo.KeyStub.TgtgKey), signinfo.Session.Tlv106Key));

                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(appInfo.WtLoginSdk.MiscBitmap | (uint) WtLoginSigType.WLOGIN_DA2,
                        appInfo.WtLoginSdk.SubSigBitmap, appInfo.WtLoginSdk.SubAppIdList)));
                    tlvs.PutTlv(new Tlv(0x0100, new T100Body(appInfo.AppId,
                        appInfo.SubAppId, appInfo.AppClientVersion, 34869472)));
                    tlvs.PutTlv(new Tlv(0x0107, new T107Body()));
                    tlvs.PutTlv(new Tlv(0x0142, new T142Body(appInfo.ApkPackageName)));

                    tlvs.PutTlv(new Tlv(0x0144, new T144Body(device.System.AndroidId,
                        report,
                        device.System.Type,
                        device.System.Version,
                        device.Network.NetType,
                        device.Network.NetOperator,
                        device.Network.NetApn,
                        true, true, false,
                        device.System.Guid,
                        285212672,
                        device.Model.Name,
                        device.Model.Manufacturer), signinfo.KeyStub.TgtgKey));
                    tlvs.PutTlv(new Tlv(0x0145, new T145Body(device.System.Guid)));
                    tlvs.PutTlv(new Tlv(0x0147, new T147Body(appInfo.AppId,
                        appInfo.ApkVersionName, appInfo.ApkSignatureMd5)));
                    // tlvs.PushTlv(new 166());
                    tlvs.PutTlv(new Tlv(0x0154, new T154Body(sequence)));
                    tlvs.PutTlv(new Tlv(0x0141, new T141Body(device.Network.NetOperator,
                        device.Network.NetType, device.Network.NetApn)));
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body()));
                    tlvs.PutTlv(new Tlv(0x0511, new T511Body(new[]
                    {
                        "office.qq.com",
                        "qun.qq.com",
                        "gamecenter.qq.com",
                        "docs.qq.com",
                        "mail.qq.com",
                        "ti.qq.com",
                        "vip.qq.com",
                        "tenpay.com",
                        "qqweb.qq.com",
                        "qzone.qq.com",
                        "mma.qq.com",
                        "game.qq.com",
                        "openmobile.qq.com",
                        "connect.qq.com",
                    })));
                    tlvs.PutTlv(new Tlv(0x0187, new T187Body(device.Network.WifiMacAddress, 0)));
                    tlvs.PutTlv(new Tlv(0x0188, new T188Body(device.System.AndroidId)));
                    tlvs.PutTlv(new Tlv(0x0194, new T194Body(device.Model.Imsi)));
                    tlvs.PutTlv(new Tlv(0x0191, new T191Body()));
                    tlvs.PutTlv(new Tlv(0x0202, new T202Body(device.Network.WifiBssid,
                        device.Network.WifiSsid)));
                    tlvs.PutTlv(new Tlv(0x0177, new T177Body(appInfo
                        .WtLoginSdk.SdkBuildTime, appInfo.WtLoginSdk.SdkVersion)));
                    tlvs.PutTlv(new Tlv(0x0516, new T516Body()));
                    tlvs.PutTlv(new Tlv(0x0521, new T521Body()));
                    tlvs.PutTlv(new Tlv(0x0525, new T525Body(new Tlv(0x0536,
                        new T536Body(new byte[] {0x01, 0x00})))));
                    tlvs.PutTlv(new Tlv(0x0544, new T544Body(OicqSubCommand, 2, 
                        appInfo.WtLoginSdk.SdkVersion, device.System.Guid, signinfo.Account.Uin)));
                    
                    if (appInfo.ApkPackageName == "com.tencent.mobileqq")
                        tlvs.PutTlv(new Tlv(0x0545, new T545Body(device, appInfo)));
                }

                w.PutUshortBE(OicqSubCommand);
                w.PutBytes(tlvs.GetBytes(true));
            })
    {
    }
}
