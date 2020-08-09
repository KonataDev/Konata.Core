using System.IO;
using System.Text;
using Konata.Protocol.Packet.Tlvs;
using Konata.Protocol.Protobuf;
using Konata.Protocol.Utils;
using Konata.Utils;
using Konata.Utils.Crypto;
using ProtoBuf;

namespace Konata.Protocol.Packet.Oicq
{
    public class OicqRequestTgtgt : OicqRequest
    {

        private ulong _uin;
        private string _password;
        private byte[] _tgtgKey;

        public OicqRequestTgtgt(ulong botUin, string botPassword, byte[] tgtgKey)
        {
            _cmd = 0x0810;
            _subCmd = 0x09;
            _serviceCmd = "wtlogin.login";

            _uin = botUin;
            _password = botPassword;
            _tgtgKey = tgtgKey;
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
            Serializer.Serialize(reportData, deviceReport);

            byte[] passwordMd5 = new Md5Cryptor().Encrypt(Encoding.UTF8.GetBytes(_password));

            tlvs.PushTlv(new T18(AppInfo.appId, AppInfo.appClientVersion, _uin));
            tlvs.PushTlv(new T1(_uin, DeviceInfo.Network.Wifi.IpAddress));
            tlvs.PushTlv(new T106(AppInfo.appId, AppInfo.subAppId, AppInfo.appClientVersion, _uin,
                new byte[4], true, passwordMd5, 0, _tgtgKey, true, DeviceInfo.Guid, LoginType.Password));
            tlvs.PushTlv(new T116(184024956, 66560));
            tlvs.PushTlv(new T100(AppInfo.appId, AppInfo.subAppId, AppInfo.appClientVersion));
            tlvs.PushTlv(new T142(AppInfo.apkPackageName));
            tlvs.PushTlv(new T144(DeviceInfo.System.AndroidId, reportData.ToArray(), DeviceInfo.System.Os,
                DeviceInfo.System.OsVersion, DeviceInfo.Network.Type, DeviceInfo.Network.Mobile.OperatorName,
                new byte[0], DeviceInfo.Network.Wifi.ApnName, true, true, false,
                DeviceInfo.Guid, 0, DeviceInfo.System.ModelName, DeviceInfo.System.Manufacturer, _tgtgKey));
            tlvs.PushTlv(new T145(DeviceInfo.Guid));
            tlvs.PushTlv(new T147(AppInfo.appId, AppInfo.apkVersionName, AppInfo.apkSignature));
            // tlvs.PushTlv(Tlv.166());
            tlvs.PushTlv(new T154(0));
            tlvs.PushTlv(new T141(DeviceInfo.Network.Mobile.OperatorName, DeviceInfo.Network.Type, DeviceInfo.Network.Wifi.ApnName));
            tlvs.PushTlv(new T8());
            tlvs.PushTlv(new T511(new string[]
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
            tlvs.PushTlv(new T187(DeviceInfo.Network.Wifi.MacAddress));
            tlvs.PushTlv(new T188(DeviceInfo.System.AndroidId));
            // tlvs.PushTlv(Tlv.194());
            tlvs.PushTlv(new T191());
            tlvs.PushTlv(new T202(DeviceInfo.Network.Wifi.ApMacAddress, DeviceInfo.Network.Wifi.Ssid));
            tlvs.PushTlv(new T177());
            tlvs.PushTlv(new T516());
            tlvs.PushTlv(new T521());
            tlvs.PushTlv(new T525(new T536(new byte[] { 0x01, 0x00 })));

            return tlvs.GetPacket(true);
        }
    }
}
