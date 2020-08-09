using System;
using System.IO;
using System.Linq;
using System.Text;
using Konata.Utils.Crypto;
using Konata.Protocol.Utils;
using Konata.Protocol.Protobuf;

using Guid = Konata.Utils.Guid;

namespace Konata.Protocol.Packet
{
    public static class Tlv
    {
        //public static byte[] T1(ulong uin, byte[] ipAddress)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x01);
        //    builder.PushInt16(1); // _ip_ver
        //    builder.PushInt32(new Random().Next());
        //    builder.PushInt32((int)uin);
        //    builder.PushUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        //    builder.PushBytes(ipAddress);
        //    builder.PushInt16(0);
        //    return builder.GetPacket();
        //}

        //public static byte[] T2(string captchaCode, byte[] captchaKey)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x02);
        //    builder.PushInt16(0); // _sigVer
        //    builder.PushString(captchaCode);
        //    builder.PushBytes(captchaKey, false, true);
        //    return builder.GetPacket();
        //}

        //public static byte[] T8(int localId = 2052, int timeZoneVer = 0, int timeZoneOffset = 0)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x08);
        //    builder.PushInt16((short)timeZoneVer);
        //    builder.PushInt32(localId);
        //    builder.PushInt16((short)timeZoneOffset);
        //    return builder.GetPacket();
        //}

        //public static byte[] T18(long appId, int appClientVersion, ulong uin, int preservedBeZero = 0)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x18);
        //    builder.PushInt16(1); // _ping_version
        //    builder.PushInt32(1536); // _sso_version
        //    builder.PushInt32((int)appId);
        //    builder.PushInt32(appClientVersion);
        //    builder.PushInt32((int)uin);
        //    builder.PushInt16((short)preservedBeZero);
        //    builder.PushInt16(0);
        //    return builder.GetPacket();
        //}

        // 未完成
        //public static byte[] T104(string sigSession)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x104);
        //    return builder.GetPacket();
        //}

        public static byte[] T106(long appId, long subAppId, int appClientVersion,
           ulong uin, byte[] ipAddress, bool isSavePassword, byte[] passwordMd5, ulong salt,
            byte[] tgtgKey, bool isGuidAvailable, byte[] guid, LoginType loginType)
        {
            TlvBuilder builder = new TlvBuilder(0x106);
            builder.PushInt16(4); // _TGTGTVer
            builder.PushInt32(new Random().Next());
            builder.PushInt32(6); // _SSoVer
            builder.PushInt32((int)appId);
            builder.PushInt32(appClientVersion);
            builder.PushUInt64(uin == 0 ? salt : uin);
            builder.PushUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            builder.PushBytes(ipAddress);
            builder.PushBool(isSavePassword);
            builder.PushBytes(passwordMd5, false);
            builder.PushBytes(tgtgKey, false);
            builder.PushInt32(0);
            builder.PushBool(isGuidAvailable);
            builder.PushBytes(isGuidAvailable ? guid : Guid.Generate(), false);
            builder.PushInt32((int)subAppId);
            builder.PushInt32((int)loginType);
            builder.PushString(uin.ToString());
            builder.PushInt16(0);

            byte[] cryptKey = new Md5Cryptor().Encrypt(passwordMd5.Concat(BitConverter.GetBytes(uin).Reverse().ToArray()).ToArray());

            return builder.GetEnctyptedPacket(new TeaCryptor(), cryptKey);
        }

        //public static byte[] T100(long appId, long subAppId, int appClientVersion)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x100);
        //    builder.PushInt16(1); // _db_buf_ver
        //    builder.PushInt32(6); // _sso_ver
        //    builder.PushInt32((int)appId);
        //    builder.PushInt32((int)subAppId);
        //    builder.PushInt32(appClientVersion);
        //    builder.PushInt32(34869472); // sigmap
        //    return builder.GetPacket();
        //}

        //public static byte[] T107(int picType, int capType = 0, int picSize = 0, int retType = 1)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x107);
        //    builder.PushInt16((short)picType);
        //    builder.PushInt8((sbyte)capType);
        //    builder.PushInt16((short)picSize);
        //    builder.PushInt8((sbyte)retType);
        //    return builder.GetPacket();
        //}

        //public static byte[] T108(byte[] ksid)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x108);
        //    builder.PushBytes(ksid);
        //    return builder.GetPacket();
        //}

        // 尚未測試
        //public static byte[] T109(string osType)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x109);
        //    builder.PushString(osType, false);
        //    return builder.GetPacket();
        //}

        //public static byte[] T116(int bitmap, int getSig, long[] subAppIdList = null)
        //{
        //    if (subAppIdList == null)
        //        subAppIdList = new long[] { 1600000226L };

        //    TlvBuilder builder = new TlvBuilder(0x116);
        //    builder.PushInt8(0); // _ver
        //    builder.PushInt32(bitmap);
        //    builder.PushInt32(getSig);
        //    builder.PushInt8((sbyte)subAppIdList.Length);
        //    foreach (long element in subAppIdList)
        //    {
        //        builder.PushInt32((int)element);
        //    }
        //    return builder.GetPacket();
        //}

        //public static byte[] T124(string osType, string osVersion, NetworkType networkType,
        //    string networkDetail, byte[] unknownZeroBytes, string apnName)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x124);
        //    builder.PushString(osType, true, true, 16);
        //    builder.PushString(osVersion, true, true, 16);
        //    builder.PushInt16((short)networkType);
        //    builder.PushString(networkDetail, true, true, 16);
        //    builder.PushBytes(unknownZeroBytes, false, true, true, 32);
        //    builder.PushString(apnName, true, true, 16);
        //    return builder.GetPacket();
        //}

        //public static byte[] T128(bool isNewInstall, bool isGuidAvaliable, bool isGuidChanged,
        //    byte[] guid, int guidFlag, string deviceModel, string deviceBrand)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x128);
        //    builder.PushInt16(0);
        //    builder.PushBool(isNewInstall);
        //    builder.PushBool(isGuidAvaliable);
        //    builder.PushBool(isGuidChanged);
        //    builder.PushInt32(guidFlag);
        //    builder.PushString(deviceModel, true, true, 32);
        //    builder.PushBytes(guid, false, true, true, 16);
        //    builder.PushString(deviceBrand, true, true, 16);
        //    return builder.GetPacket();
        //}

        //public static byte[] T142(string apkId)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x142);
        //    builder.PushInt16(0); // _version
        //    builder.PushString(apkId, true, true, 32);
        //    return builder.GetPacket();
        //}

        //public static byte[] T141(string simOperatorName, NetworkType networkType, string apnName)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x141);
        //    builder.PushInt16(1); // _version
        //    builder.PushString(simOperatorName);
        //    builder.PushInt16((short)networkType);
        //    builder.PushString(apnName);
        //    return builder.GetPacket();
        //}

        // 尚未測試
        public static byte[] T144(string androidId, byte[] deviceDevInfo,
            string osType, string osVersion, NetworkType networkType, string networkDetail, byte[] unknownZeroBytes, string apnName,
            bool isNewInstall, bool isGuidAvaliable, bool isGuidChanged, byte[] guid, int guidFlag, string deviceModel, string deviceBrand,
            byte[] tgtgKey)
        {
            return T144(
                T109(androidId),
                T52d(deviceDevInfo),
                T124(osType, osVersion, networkType, networkDetail, unknownZeroBytes, apnName),
                T128(isNewInstall, isGuidAvaliable, isGuidChanged, guid, guidFlag, deviceModel, deviceBrand),
                null, tgtgKey
                );
        }

        // 尚未測試
        public static byte[] T144(byte[] tlv109, byte[] tlv52d, byte[] tlv124, byte[] tlv128,
            byte[] tlv16e, byte[] tgtgKey)
        {
            return T144(
                tlv109, tlv52d, tlv124, tlv128,
                null, null, tlv16e, tgtgKey
                );
        }

        // 尚未測試
        public static byte[] T144(byte[] tlv109, byte[] tlv52d, byte[] tlv124, byte[] tlv128,
            byte[] tlv148, byte[] tlv153, byte[] tlv16e, byte[] tgtgKey)
        {
            TlvBuilder builder = new TlvBuilder(0x144);

            TlvPacker packer = new TlvPacker();
            packer.PushTlv(tlv109);
            packer.PushTlv(tlv52d);
            packer.PushTlv(tlv124);
            packer.PushTlv(tlv109);
            packer.PushTlv(tlv128);
            packer.PushTlv(tlv148);
            packer.PushTlv(tlv153);
            packer.PushTlv(tlv16e);
            builder.PushBytes(packer.GetEncryptedPacket(true, new TeaCryptor(), tgtgKey), false);

            return builder.GetPacket();
        }

        //public static byte[] T145(byte[] guid)
        //{
        //    TlvBuilder builder = new TlvBuilder(0x145);
        //    builder.PushBytes(guid);
        //    return builder.GetPacket();
        //}

        public static byte[] T147(long appId, string apkVersionName, byte[] apkSignatureMd5)
        {
            TlvBuilder builder = new TlvBuilder(0x147);
            builder.PushInt32((int)appId);
            builder.PushString(apkVersionName, true, true, 32);
            builder.PushBytes(apkSignatureMd5, false, true, true, 32);
            return builder.GetPacket();
        }

        // 尚未測試
        public static byte[] T148(string appName, long ssoVersion, long appId, long subAppId,
            string appVersion, string appSignature)
        {
            TlvBuilder builder = new TlvBuilder(0x148);
            builder.PushString(appName);
            builder.PushInt32((int)ssoVersion);
            builder.PushInt32((int)appId);
            builder.PushInt32((int)subAppId);
            builder.PushString(appVersion);
            builder.PushString(appSignature);
            return builder.GetPacket();
        }

        /*
        public static byte[] T153(bool isRooted)
        {
            TlvBuilder builder = new TlvBuilder(0x153);
            builder.PushBool(isRooted, 2);
            return builder.GetPacket();
        }
      

        public static byte[] T154(int ssoSequenceId)
        {
            TlvBuilder builder = new TlvBuilder(0x154);
            builder.PushInt32(ssoSequenceId);
            return builder.GetPacket();
        }
 
        public static byte[] T16e(string deviceName)
        {
            TlvBuilder builder = new TlvBuilder(0x16e);
            builder.PushString(deviceName, false);
            return builder.GetPacket();
        }
 */
        public static byte[] T177(long buildTime = 1577331209, string sdkVersion = "6.0.0.2425")
        {
            TlvBuilder builder = new TlvBuilder(0x177);
            builder.PushInt8(1);
            builder.PushInt32((int)buildTime);
            builder.PushString(sdkVersion);
            return builder.GetPacket();
        }

        /*
        public static byte[] T187(byte[] macAddress)
        {
            TlvBuilder builder = new TlvBuilder(0x187);
            builder.PushBytes(new Md5Cryptor().Encrypt(macAddress));
            return builder.GetPacket();
        }
        public static byte[] T188(byte[] androidId)
        {
            TlvBuilder builder = new TlvBuilder(0x188);
            builder.PushBytes(new Md5Cryptor().Encrypt(androidId));
            return builder.GetPacket();
        }


        public static byte[] T191(int unknownK = 0x82)
        {
            TlvBuilder builder = new TlvBuilder(0x191);
            builder.PushUInt8((byte)unknownK);
            return builder.GetPacket();
        }

        public static byte[] T192(string url)
        {
            TlvBuilder builder = new TlvBuilder(0x192);
            builder.PushString(url, false);
            return builder.GetPacket();
        }
*/

        public static byte[] T202(byte[] wifiBssid, string wifiSsid)
        {
            TlvBuilder builder = new TlvBuilder(0x202);
            builder.PushBytes(new Md5Cryptor().Encrypt(wifiBssid), false, true, true, 16);
            builder.PushString(wifiSsid, true, true, 32);
            return builder.GetPacket();
        }
/*
        public static byte[] T318(byte[] tgtQr)
        {
            TlvBuilder builder = new TlvBuilder(0x318);
            builder.PushBytes(tgtQr, false);
            return builder.GetPacket();
        }
*/
        public static byte[] T511(string[] domains)
        {
            TlvBuilder builder = new TlvBuilder(0x511);
            builder.PushUInt16((ushort)domains.Length);
            foreach (string element in domains)
            {
                builder.PushInt8(1);
                builder.PushString(element);
            }
            return builder.GetPacket();
        }
        /*
        public static byte[] T516(int sourceType = 0)
        {
            TlvBuilder builder = new TlvBuilder(0x516);
            builder.PushInt32(sourceType);
            return builder.GetPacket();
        }
        */
        public static byte[] T521(int productType = 0, short unknown = 0)
        {
            TlvBuilder builder = new TlvBuilder(0x521);
            builder.PushInt32(productType);
            builder.PushInt16(unknown);
            return builder.GetPacket();
        }

        public static byte[] T525(byte[] t536Data)
        {
            TlvBuilder builder = new TlvBuilder(0x525);
            builder.PushInt16(1);
            builder.PushBytes(t536Data, false);
            return builder.GetPacket();
        }


        // 尚未測試
        public static byte[] T52d(byte[] deviceReportInfo)
        {
            TlvBuilder builder = new TlvBuilder(0x52d);
            builder.PushBytes(deviceReportInfo, false);
            return builder.GetPacket();
        }

        // 尚未測試
        public static byte[] T52d(string bootLoader, string version, string codeName, string incremental,
            string fingerprint, string bootId, string androidId, string baseBand, string innerVersion)
        {
            DeviceReport report = new DeviceReport
            {
                Bootloader = Encoding.UTF8.GetBytes(bootLoader),
                Version = Encoding.UTF8.GetBytes(version),
                CodeName = Encoding.UTF8.GetBytes(codeName),
                Incremental = Encoding.UTF8.GetBytes(incremental),
                Fingerprint = Encoding.UTF8.GetBytes(fingerprint),
                BootId = Encoding.UTF8.GetBytes(bootId),
                AndroidId = Encoding.UTF8.GetBytes(androidId),
                BaseBand = Encoding.UTF8.GetBytes(baseBand),
                InnerVersion = Encoding.UTF8.GetBytes(innerVersion)
            };

            MemoryStream stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, report);

            TlvBuilder builder = new TlvBuilder(0x52d);
            builder.PushBytes(stream.ToArray(), false);
            return builder.GetPacket();
        }

        /*
        public static byte[] T536(byte[] loginExtraData)
        {
            TlvBuilder builder = new TlvBuilder(0x536);
            builder.PushBytes(loginExtraData, false);
            return builder.GetPacket();
        }
        */
    }

}
