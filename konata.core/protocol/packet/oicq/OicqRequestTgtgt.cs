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

        private long _uin;
        private string _password;
        private byte[] _tgtgKey;
        private byte[] _randKey;
        private byte[] _shareKey;
        private byte[] _publicKey;

        public OicqRequestTgtgt(long uin, string botPassword, byte[] tgtgKey, byte[] randKey, byte[] shareKey, byte[] publicKey)
        {
            _cmd = 0x0810;
            _subCmd = 0x09;

            _uin = uin;
            _password = botPassword;
            _tgtgKey = tgtgKey;
            _randKey = randKey;
            _shareKey = shareKey;
            _publicKey = publicKey;
        }

        public override byte[] GetBytes()
        {

            DeviceReport deviceReport = new DeviceReport
            {
                Bootloader = Encoding.UTF8.GetBytes(DeviceInfo.Build.Bootloader),
                Version = new byte[0], // Encoding.UTF8.GetBytes(DeviceInfo.System.OsVersion),
                CodeName = Encoding.UTF8.GetBytes(DeviceInfo.Build.CodeName),
                Incremental = Encoding.UTF8.GetBytes(DeviceInfo.Build.Incremental),
                Fingerprint = Encoding.UTF8.GetBytes(DeviceInfo.Build.Fingerprint),
                BootId = new byte[0], // Encoding.UTF8.GetBytes(DeviceInfo.BootId),
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
            tlvs.PushTlv(new T107(0, 0, 0));
            tlvs.PushTlv(new T142(AppInfo.apkPackageName));
            tlvs.PushTlv(new T144(DeviceInfo.System.AndroidId, reportData.ToArray(), DeviceInfo.System.Os,
                DeviceInfo.System.OsVersion, DeviceInfo.Network.Type, DeviceInfo.Network.Mobile.OperatorName,
                DeviceInfo.Network.Wifi.ApnName, true, true, false, DeviceInfo.Guid, 285212672,
                DeviceInfo.System.ModelName, DeviceInfo.System.Manufacturer, _tgtgKey));
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
            // tlvs.PushTlv(new T544());
            // tlvs.PushTlv(new T545());

            // 構建 tlv包
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt16(_subCmd);
            builder.PushBytes(tlvs.GetPacket(true), false);
            var tlvBody = builder.GetEncryptedBytes(new TeaCryptor(), _shareKey);

            // 構建 密鑰
            builder.PushInt16(0x0101);
            builder.PushBytes(_randKey, false);
            builder.PushInt16(0x0102);
            builder.PushBytes(_publicKey, false, true);
            var keyBody = builder.GetBytes();

            // 構建 oicq_request
            StreamBuilder request = new StreamBuilder();
            request.PushInt8(0x02); // 頭部 0x02
            request.PushUInt16((ushort)(27 + 2 + keyBody.Length + tlvBody.Length));
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

            request.PushBytes(keyBody, false);
            request.PushBytes(tlvBody, false);
            request.PushInt8(0x03); // 尾部 0x03

            return request.GetBytes();
        }
    }
}
