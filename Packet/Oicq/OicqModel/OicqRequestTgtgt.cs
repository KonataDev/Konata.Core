using System;

using Konata.Core;
using Konata.Core.Service;
using Konata.Core.Packet.Tlv;
using Konata.Core.Packet.Tlv.TlvModel;
using Konata.Core.Packet.Protobuf;

namespace Konata.Core.Packet.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestTgtgt : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0009;

        public OicqRequestTgtgt(int sequence, SignInfo signinfo, BotDevice device)
            : base(OicqCommand, OicqSubCommand, signinfo.UinInfo.Uin, OicqEncryptMethod.ECDH135,
                  new XTGTGT(signinfo.UinInfo.Uin, sequence, signinfo, device),
                  signinfo.ShareKey, signinfo.RandKey, signinfo.DefaultPublicKey)
        {

        }

        public class XTGTGT : OicqRequestBody
        {
            public XTGTGT(uint uin, int ssoSequence, SignInfo signinfo, BotDevice device)
                : base()
            {
                // 設備訊息上報
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

                // 構建 tlv
                TlvPacker tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0018, new T18Body(AppInfo.AppId, AppInfo.AppClientVersion, uin)));
                    tlvs.PutTlv(new Tlv(0x0001, new T1Body(uin, device.Network.Wifi.IpAddress)));

                    tlvs.PutTlv(new Tlv(0x0106, new T106Body(AppInfo.AppId, AppInfo.SubAppId, AppInfo.AppClientVersion,
                        uin, new byte[4], true, signinfo.PasswordMd5, 0, true, device.Guid, LoginType.Password,
                        signinfo.TgtgKey), signinfo.Tlv106Key));

                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(AppInfo.WtLoginSdk.MiscBitmap | (uint)WtLoginSigType.WLOGIN_DA2,
                        AppInfo.WtLoginSdk.SubSigBitmap, AppInfo.WtLoginSdk.SubAppIdList)));

                    tlvs.PutTlv(new Tlv(0x0100, new T100Body(AppInfo.AppId, AppInfo.SubAppId, AppInfo.AppClientVersion)));

                    tlvs.PutTlv(new Tlv(0x0107, new T107Body()));
                    tlvs.PutTlv(new Tlv(0x0142, new T142Body(AppInfo.ApkPackageName)));

                    tlvs.PutTlv(new Tlv(0x0144, new T144Body(device.System.AndroidId,
                        report, device.System.Type, device.System.Version,
                        device.Network.Type, device.Network.Mobile.Operator,
                        device.Network.Wifi.Apn, true, true, false,
                        device.Guid, 285212672, device.Model.Name,
                        device.Model.Manufacturer), signinfo.TgtgKey));

                    tlvs.PutTlv(new Tlv(0x0145, new T145Body(device.Guid)));

                    tlvs.PutTlv(new Tlv(0x0147, new T147Body(AppInfo.AppId, AppInfo.ApkVersionName, AppInfo.ApkSignatureMd5)));
                    // tlvs.PushTlv(new 166());

                    tlvs.PutTlv(new Tlv(0x0154, new T154Body(ssoSequence)));

                    tlvs.PutTlv(new Tlv(0x0141, new T141Body(device.Network.Mobile.Operator,
                        device.Network.Type, device.Network.Wifi.Apn)));

                    tlvs.PutTlv(new Tlv(0x0008, new T8Body()));
                    tlvs.PutTlv(new Tlv(0x0511, new T511Body(new string[]
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

                    tlvs.PutTlv(new Tlv(0x0187, new T187Body(device.Network.Wifi.MacAddress, 0)));
                    tlvs.PutTlv(new Tlv(0x0188, new T188Body(device.System.AndroidId)));
                    tlvs.PutTlv(new Tlv(0x0194, new T194Body(device.Model.IMSI)));
                    tlvs.PutTlv(new Tlv(0x0191, new T191Body()));

                    tlvs.PutTlv(new Tlv(0x0202, new T202Body(device.Network.Wifi.Bssid,
                        device.Network.Wifi.Ssid)));

                    tlvs.PutTlv(new Tlv(0x0177, new T177Body(AppInfo.WtLoginSdk.SdkBuildTime, AppInfo.WtLoginSdk.SdkVersion)));

                    tlvs.PutTlv(new Tlv(0x0516, new T516Body()));
                    tlvs.PutTlv(new Tlv(0x0521, new T521Body()));

                    tlvs.PutTlv(new Tlv(0x0525, new T525Body(new Tlv(0x0536,
                        new T536Body(new byte[] { 0x01, 0x00 })))));
                    // tlvs.PushTlv(new T544());
                    // tlvs.PushTlv(new T545());
                }

                PutUshortBE(OicqSubCommand);
                PutBytes(tlvs.GetBytes(true));
            }
        }
    }
}
