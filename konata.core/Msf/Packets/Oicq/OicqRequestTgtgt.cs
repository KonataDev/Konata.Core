using System.IO;
using System.Text;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;
using Konata.Msf.Packets.Tlvs;
using Konata.Msf.Packets.Protobuf;
using ProtoBuf;

namespace Konata.Msf.Packets.Oicq
{
    public class OicqRequestTgtgt : OicqRequest
    {

        private uint _uin;
        private string _password;
        private byte[] _tgtgKey;
        private byte[] _randKey;
        private byte[] _shareKey;
        private byte[] _publicKey;

        public OicqRequestTgtgt(uint uin, string botPassword, byte[] tgtgKey, byte[] randKey, byte[] shareKey, byte[] publicKey)
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

        public byte[] GetBytes()
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
            tlvs.PutTlv(new T18(AppInfo.appId, AppInfo.appClientVersion, _uin));
            tlvs.PutTlv(new T1(_uin, DeviceInfo.Network.Wifi.IpAddress));
            tlvs.PutTlv(new T106(AppInfo.appId, AppInfo.subAppId, AppInfo.appClientVersion, _uin,
                new byte[4], true, passwordMd5, 0, _tgtgKey, true, DeviceInfo.Guid, LoginType.Password));
            tlvs.PutTlv(new T116(184024956, 66560));
            tlvs.PutTlv(new T100(AppInfo.appId, AppInfo.subAppId, AppInfo.appClientVersion));
            tlvs.PutTlv(new T107(0, 0, 0));
            tlvs.PutTlv(new T142(AppInfo.apkPackageName));
            tlvs.PutTlv(new T144(DeviceInfo.System.AndroidId, reportData.ToArray(), DeviceInfo.System.Os,
                DeviceInfo.System.OsVersion, DeviceInfo.Network.Type, DeviceInfo.Network.Mobile.OperatorName,
                DeviceInfo.Network.Wifi.ApnName, true, true, false, DeviceInfo.Guid, 285212672,
                DeviceInfo.System.ModelName, DeviceInfo.System.Manufacturer, _tgtgKey));
            tlvs.PutTlv(new T145(DeviceInfo.Guid));
            tlvs.PutTlv(new T147(AppInfo.appId, AppInfo.apkVersionName, AppInfo.apkSignature));
            // tlvs.PushTlv(new 166());
            tlvs.PutTlv(new T154(0));
            tlvs.PutTlv(new T141(DeviceInfo.Network.Mobile.OperatorName, DeviceInfo.Network.Type, DeviceInfo.Network.Wifi.ApnName));
            tlvs.PutTlv(new T8());
            tlvs.PutTlv(new T511(new string[]
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
            tlvs.PutTlv(new T187(DeviceInfo.Network.Wifi.MacAddress));
            tlvs.PutTlv(new T188(DeviceInfo.System.AndroidId));
            // tlvs.PushTlv(Tlv.194());
            tlvs.PutTlv(new T191());
            tlvs.PutTlv(new T202(DeviceInfo.Network.Wifi.ApMacAddress, DeviceInfo.Network.Wifi.Ssid));
            tlvs.PutTlv(new T177(AppInfo.WtLoginSdk.buildTime, AppInfo.WtLoginSdk.sdkVersion));
            tlvs.PutTlv(new T516());
            tlvs.PutTlv(new T521());
            tlvs.PutTlv(new T525(new T536(new byte[] { 0x01, 0x00 })));
            // tlvs.PushTlv(new T544());
            // tlvs.PushTlv(new T545());

            // 構建 tlv包
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(_subCmd);
            builder.PutBytes(tlvs.GetBytes(true));
            var tlvBody = builder.GetEncryptedBytes(new TeaCryptor(), _shareKey);

            // 構建 密鑰
            builder.PutUshortBE(0x0101);
            builder.PutBytes(_randKey);
            builder.PutUshortBE(0x0102);
            builder.PutBytes(_publicKey, 2);
            var keyBody = builder.GetBytes();

            // 構建 oicq_request
            StreamBuilder request = new StreamBuilder();
            request.PutByte(0x02); // 頭部 0x02
            request.PutUshortBE((ushort)(27 + 2 + keyBody.Length + tlvBody.Length));
            request.PutUshortBE(8001); // 協議版本 1F 41
            request.PutUshortBE(_cmd);
            request.PutUshortBE(1);
            request.PutUintBE(_uin);
            request.PutByte(0x03);
            request.PutByte(0x87); // 加密方式id
            request.PutByte(0x00); // 永遠0
            request.PutUintBE(2);
            request.PutUintBE(AppInfo.appClientVersion);
            request.PutUintBE(0);

            request.PutBytes(keyBody);
            request.PutBytes(tlvBody);
            request.PutByte(0x03); // 尾部 0x03

            return request.GetBytes();
        }
    }
}
