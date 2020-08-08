using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konata.Protocol.Packet.Tlvs;
using Konata.Protocol.Protobuf;
using Konata.Protocol.Utils;
using Konata.Utils;
using Konata.Utils.Crypto;

namespace Konata.Protocol.Packet.Oicq
{
    public class OicqRequestTgtgt : OicqRequest
    {

        private ulong uin;
        private string password;
        private byte[] tgtgKey;

        public OicqRequestTgtgt(ulong botUin, string botPassword, byte[] tgtgKey)
        {
            cmd = 0x0810;
            subCmd = 0x09;
            serviceCmd = "wtlogin.login";

            this.uin = botUin;
            this.password = botPassword;
            this.tgtgKey = tgtgKey;


        }

        public override byte[] GetBytes()
        {
            StreamBuilder builder = new StreamBuilder();
            TlvPacker tlvs = new TlvPacker();

            DeviceReport deviceReport = new DeviceReport();
            deviceReport.Bootloader = Encoding.UTF8.GetBytes(DeviceInfo.Build.Bootloader);
            deviceReport.Version = Encoding.UTF8.GetBytes(DeviceInfo.System.OsVersion);
            deviceReport.CodeName = Encoding.UTF8.GetBytes(DeviceInfo.Build.CodeName);
            deviceReport.Incremental = Encoding.UTF8.GetBytes(DeviceInfo.Build.Incremental);
            deviceReport.Fingerprint = Encoding.UTF8.GetBytes(DeviceInfo.Build.Fingerprint);
            deviceReport.BootId = Encoding.UTF8.GetBytes(DeviceInfo.BootId);
            deviceReport.AndroidId = Encoding.UTF8.GetBytes(DeviceInfo.System.AndroidId);
            deviceReport.BaseBand = Encoding.UTF8.GetBytes(DeviceInfo.Build.BaseBand);
            deviceReport.InnerVersion = Encoding.UTF8.GetBytes(DeviceInfo.Build.InnerVersion);

            MemoryStream reportData = new MemoryStream();
            ProtoBuf.Serializer.Serialize(reportData, deviceReport);

            byte[] passwordMd5 = new Md5Cryptor().Encrypt(Encoding.UTF8.GetBytes(password));

            tlvs.PushTlv(Tlv.T18(AppInfo.appId, AppInfo.appClientVersion, uin));
            tlvs.PushTlv(new T1(uin, DeviceInfo.Network.Wifi.IpAddress));
            tlvs.PushTlv(Tlv.T106(AppInfo.appId, AppInfo.subAppId, AppInfo.appClientVersion, uin,
                new byte[4], true, passwordMd5, 0, tgtgKey, true, DeviceInfo.Guid, LoginType.Password));
            tlvs.PushTlv(Tlv.T116(184024956, 66560));
            tlvs.PushTlv(Tlv.T100(AppInfo.appId, AppInfo.subAppId, AppInfo.appClientVersion));
            tlvs.PushTlv(Tlv.T142(AppInfo.apkPackageName));
            tlvs.PushTlv(Tlv.T144(DeviceInfo.System.AndroidId, reportData.ToArray(), DeviceInfo.System.Os,
                DeviceInfo.System.OsVersion, DeviceInfo.Network.Type, DeviceInfo.Network.Mobile.OperatorName,
                new byte[0], DeviceInfo.Network.Wifi.ApnName, true, true, false,
                DeviceInfo.Guid, 0, DeviceInfo.System.ModelName, DeviceInfo.System.Manufacturer, tgtgKey));
            tlvs.PushTlv(Tlv.T145(DeviceInfo.Guid));
            tlvs.PushTlv(Tlv.T147(AppInfo.appId, AppInfo.apkVersionName, AppInfo.apkSignature));
            // tlvs.PushTlv(Tlv.166());
            tlvs.PushTlv(Tlv.T154(0));
            tlvs.PushTlv(Tlv.T141(DeviceInfo.Network.Mobile.OperatorName, DeviceInfo.Network.Type, DeviceInfo.Network.Wifi.ApnName));
            tlvs.PushTlv(Tlv.T8());
            tlvs.PushTlv(Tlv.T511(new string[]
            {
                "game.qq.com",
                "mail.qq.com",
                "qzone.qq.com",
                "qun.qq.com",
                "openmobile.qq.com",
                "tenpay.com",
                "connect.qq.com",
                "qun.qq.com",
                "qqweb.qq.com",
                "office.qq.com",
                "ti.qq.com",
                "mma.qq.com",
                "docs.qq.com",
                "vip.qq.com",
                "gamecenter.qq.com",
            }));
            tlvs.PushTlv(Tlv.T187(DeviceInfo.Network.Wifi.MacAddress));
            tlvs.PushTlv(Tlv.T188(Hex.HexStr2Bytes(DeviceInfo.System.AndroidId)));
            // tlvs.PushTlv(Tlv.194());
            tlvs.PushTlv(Tlv.T191());
            tlvs.PushTlv(Tlv.T202(DeviceInfo.Network.Wifi.ApMacAddress, DeviceInfo.Network.Wifi.Ssid));
            tlvs.PushTlv(Tlv.T177());
            tlvs.PushTlv(Tlv.T516());
            tlvs.PushTlv(Tlv.T521());
            tlvs.PushTlv(Tlv.T525(Tlv.T536(new byte[] { 0x01, 0x00 })));


            return tlvs.GetPacket(true);
        }

    }
}
