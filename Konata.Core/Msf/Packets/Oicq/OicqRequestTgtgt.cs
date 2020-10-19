using System.IO;
using System.Text;
using Konata.Msf.Crypto;
using Konata.Msf.Packets.Tlv;
using Konata.Msf.Packets.Protobuf;

namespace Konata.Msf.Packets.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestTgtgt : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0009;

        public OicqRequestTgtgt(uint uin, uint ssoseq, UserSigInfo sigInfo)
            : base(OicqCommand, OicqSubCommand, uin, OicqEncryptMethod.ECDH135,
                new XTGTGT(uin, ssoseq, sigInfo.PasswordMd5, sigInfo.TgtgKey,
                    sigInfo.Tlv106Key), sigInfo.ShareKey, sigInfo.RandKey, sigInfo.DefaultPublicKey)
        {

        }

        public class XTGTGT : OicqRequestBody
        {
            public XTGTGT(uint uin, uint ssoseq, byte[] passwordMd5, byte[] tgtgKey, byte[] t106Key)
                : base()
            {
                // 設備訊息上報
                var report = new DeviceReport (
                    DeviceInfo.Build.Bootloader,
                    DeviceInfo.System.OsVersion,
                    DeviceInfo.Build.CodeName,
                    DeviceInfo.Build.Incremental,
                    DeviceInfo.Build.Fingerprint,
                    DeviceInfo.BootId,
                    DeviceInfo.System.AndroidId,
                    DeviceInfo.Build.BaseBand,
                    DeviceInfo.Build.InnerVersion);

                // 構建 tlv
                TlvPacker tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0018, new T18Body(AppInfo.appId, AppInfo.appClientVersion, uin)));
                    tlvs.PutTlv(new Tlv(0x0001, new T1Body(uin, DeviceInfo.Network.Wifi.IpAddress)));

                    tlvs.PutTlv(new Tlv(0x0106, new T106Body(AppInfo.appId, AppInfo.subAppId, AppInfo.appClientVersion, uin,
                        new byte[4], true, passwordMd5, 0, true, DeviceInfo.Guid, LoginType.Password, tgtgKey), t106Key));

                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(AppInfo.wtLoginMiscBitmap | (uint)WtLoginSigType.WLOGIN_DA2,
                        AppInfo.wtLoginSubSigBitmap, AppInfo.wtLoginSubAppIdList)));

                    tlvs.PutTlv(new Tlv(0x0100, new T100Body(AppInfo.appId, AppInfo.subAppId,
                        AppInfo.appClientVersion)));

                    tlvs.PutTlv(new Tlv(0x0107, new T107Body()));
                    tlvs.PutTlv(new Tlv(0x0142, new T142Body(AppInfo.apkPackageName)));

                    tlvs.PutTlv(new Tlv(0x0144, new T144Body(DeviceInfo.System.AndroidId,
                        report, DeviceInfo.System.Os, DeviceInfo.System.OsVersion,
                        DeviceInfo.Network.Type, DeviceInfo.Network.Mobile.OperatorName,
                        DeviceInfo.Network.Wifi.ApnName, true, true, false,
                        DeviceInfo.Guid, 285212672, DeviceInfo.System.ModelName,
                        DeviceInfo.System.Manufacturer), tgtgKey));

                    tlvs.PutTlv(new Tlv(0x0145, new T145Body(DeviceInfo.Guid)));

                    tlvs.PutTlv(new Tlv(0x0147, new T147Body(AppInfo.appId, AppInfo.apkVersionName,
                        AppInfo.apkSignatureMd5)));
                    // tlvs.PushTlv(new 166());

                    tlvs.PutTlv(new Tlv(0x0154, new T154Body(ssoseq)));

                    tlvs.PutTlv(new Tlv(0x0141, new T141Body(DeviceInfo.Network.Mobile.OperatorName,
                        DeviceInfo.Network.Type, DeviceInfo.Network.Wifi.ApnName)));

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

                    tlvs.PutTlv(new Tlv(0x0187, new T187Body(DeviceInfo.Network.Wifi.MacAddress, 0)));
                    tlvs.PutTlv(new Tlv(0x0188, new T188Body(DeviceInfo.System.AndroidId)));
                    tlvs.PutTlv(new Tlv(0x0194, new T194Body(DeviceInfo.System.Imsi)));
                    tlvs.PutTlv(new Tlv(0x0191, new T191Body()));

                    tlvs.PutTlv(new Tlv(0x0202, new T202Body(DeviceInfo.Network.Wifi.ApMacAddress,
                        DeviceInfo.Network.Wifi.Ssid)));

                    tlvs.PutTlv(new Tlv(0x0177, new T177Body(AppInfo.WtLoginSdk.buildTime,
                        AppInfo.WtLoginSdk.sdkVersion)));

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
