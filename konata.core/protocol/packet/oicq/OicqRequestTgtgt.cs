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
        private byte[] _randKey = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private byte[] _shareKey = { 0x4D, 0xA0, 0xF6, 0x14, 0xFC, 0x9F, 0x29, 0xC2, 0x05, 0x4C, 0x77, 0x04, 0x8A, 0x65, 0x66, 0xD7 };

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

            DeviceReport deviceReport = new DeviceReport
            {
                Bootloader = Encoding.UTF8.GetBytes(DeviceInfo.Build.Bootloader),
                Version = Encoding.UTF8.GetBytes(DeviceInfo.System.OsVersion),
                CodeName = Encoding.UTF8.GetBytes(DeviceInfo.Build.CodeName),
                Incremental = Encoding.UTF8.GetBytes(DeviceInfo.Build.Incremental),
                Fingerprint = Encoding.UTF8.GetBytes(DeviceInfo.Build.Fingerprint),
                BootId = Encoding.UTF8.GetBytes(DeviceInfo.BootId),
                AndroidId = Encoding.UTF8.GetBytes(DeviceInfo.System.AndroidId),
                BaseBand = Encoding.UTF8.GetBytes(DeviceInfo.Build.BaseBand),
                InnerVersion = Encoding.UTF8.GetBytes(DeviceInfo.Build.InnerVersion)
            };
            MemoryStream reportData = new MemoryStream();
            Serializer.Serialize(reportData, deviceReport);

            byte[] passwordMd5 = new Md5Cryptor().Encrypt(Encoding.UTF8.GetBytes(_password));

            // 構建 tlv
            TlvPacker tlvs = new TlvPacker();
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
            // tlvs.PushTlv(new 166());
            tlvs.PushTlv(new T154(0));
            tlvs.PushTlv(new T141(DeviceInfo.Network.Mobile.OperatorName, DeviceInfo.Network.Type, DeviceInfo.Network.Wifi.ApnName));
            tlvs.PushTlv(new T8());
            tlvs.PushTlv(new T511(new string[]
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

            // 構建 oicq_request 包躰
            StreamBuilder requestBody = new StreamBuilder();
            requestBody.PushInt16(_subCmd);
            requestBody.PushBytes(tlvs.GetPacket(true), false);

            // 構建 oicq_request
            StreamBuilder request = new StreamBuilder();
            request.PushInt8(0x02); // 頭部 0x02
            request.PushInt16(27 + 2);
            request.PushInt16(8001); // 協議版本 1F 41
            request.PushInt16(_cmd);
            request.PushInt16(1);
            request.PushInt32((int)_uin);
            request.PushInt8(0x03);
            request.PushUInt8(0x87); // 加密方式id
            request.PushInt8(0x00); // 永遠0
            request.PushInt32(2);
            request.PushInt32(AppInfo.appClientVersion);
            request.PushInt32(0);

            // 密鑰
            request.PushInt16(0x0101);
            request.PushBytes(_randKey, false);
            request.PushInt16(0x0102);
            request.PushBytes(new byte[0], false, true);

            request.PushBytes(requestBody.GetEncryptedBytes(new TeaCryptor(), _shareKey), false);
            request.PushInt8(0x03); // 尾部 0x03

            return request.GetPlainBytes();
        }
    }
}
