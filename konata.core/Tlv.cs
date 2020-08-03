using System;
using Konata.Utils;
using Konata.Crypto;

namespace Konata
{
    static class Tlv
    {
        public static byte[] T1(ulong uin, byte[] ipAddress)
        {
            TlvBuilder builder = new TlvBuilder(0x01);
            builder.PushInt16(1); // _ip_ver
            builder.PushInt32(new Random().Next());
            builder.PushInt32((int)uin);
            builder.PushUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            builder.PushBytes(ipAddress);
            builder.PushInt16(0);
            return builder.GetPacket();
        }

        public static byte[] T8(int localId = 2052, int timeZoneVer = 0, int timeZoneOffset = 0)
        {
            TlvBuilder builder = new TlvBuilder(0x08);
            builder.PushInt16((short)timeZoneVer);
            builder.PushInt32(localId);
            builder.PushInt16((short)timeZoneOffset);
            return builder.GetPacket();
        }

        public static byte[] T018(long appId, int appClientVersion, ulong uin, int preservedBeZero = 0)
        {
            TlvBuilder builder = new TlvBuilder(0x18);
            builder.PushInt16(1); // _ping_version
            builder.PushInt32(1536); // _sso_version
            builder.PushInt32((int)appId);
            builder.PushInt32(appClientVersion);
            builder.PushInt32((int)uin);
            builder.PushInt16((short)preservedBeZero);
            builder.PushInt16(0);
            return builder.GetPacket();
        }

        // 未完成 有加密
        public static byte[] T106(long appId, long subAppId, int appClientVersion,
           ulong uin, byte[] ipAddress, bool isSavePassword, byte[] passwordMd5, ulong salt,
           byte[] uinString, byte[] tgtgKey, bool isGuidAvailable, byte[] guid, int loginType)
        {
            TlvBuilder builder = new TlvBuilder(0x106);
            builder.PushInt16(4); // _TGTGTVer
            builder.PushInt32(new Random().Next());
            builder.PushInt32(6); // _SSoVer
            builder.PushInt32((int)appId);
            builder.PushInt32(appClientVersion);
            builder.PushUInt64(uin == 0 ? salt : uin);
            //builder.PushInt8();
            builder.PushUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            //builder.PushInt8();
            builder.PushBytes(ipAddress);

            builder.PushInt32((int)subAppId);
            builder.PushInt32(loginType);
            builder.PushInt16(0);

            return builder.GetPacket();
        }

        public static byte[] T100(long appId, long subAppId, int appClientVersion)
        {
            TlvBuilder builder = new TlvBuilder(0x100);
            builder.PushInt16(1); // _db_buf_ver
            builder.PushInt32(6); // _sso_ver
            builder.PushInt32((int)appId);
            builder.PushInt32((int)subAppId);
            builder.PushInt32(appClientVersion);
            builder.PushInt32(34869472); // sigmap
            return builder.GetPacket();
        }

        public static byte[] T107(int picType, int capType = 0, int picSize = 0, int retType = 1)
        {
            TlvBuilder builder = new TlvBuilder(0x107);
            builder.PushInt16((short)picType);
            builder.PushInt8((sbyte)capType);
            builder.PushInt16((short)picSize);
            builder.PushInt8((sbyte)retType);
            return builder.GetPacket();
        }

        public static byte[] T108(byte[] ksid)
        {
            TlvBuilder builder = new TlvBuilder(0x108);
            builder.PushBytes(ksid);
            return builder.GetPacket();
        }

        public static byte[] T116(int bitmap, int getSig, long[] subAppIdList)
        {
            TlvBuilder builder = new TlvBuilder(0x116);
            builder.PushInt8(0); // _ver
            builder.PushInt32(bitmap);
            builder.PushInt32(getSig);
            builder.PushInt8((sbyte)subAppIdList.Length);
            foreach (long element in subAppIdList)
            {
                builder.PushInt32((int)element);
            }
            return builder.GetPacket();
        }

        public static byte[] T142(string apkId)
        {
            TlvBuilder builder = new TlvBuilder(0x142);
            builder.PushInt16(0); // _version
            builder.PushString(apkId, true, true, 32);
            return builder.GetPacket();
        }

        public static byte[] T141(string simOperatorName, int networkType, string apnName)
        {
            TlvBuilder builder = new TlvBuilder(0x141);
            builder.PushInt16(1); // _version
            builder.PushString(simOperatorName);
            builder.PushInt16((short)networkType);
            builder.PushString(apnName);
            return builder.GetPacket();
        }

        // 未完成 有加密
        public static byte[] T144()
        {

            return new byte[0];
        }

        public static byte[] T145(byte[] guid)
        {
            TlvBuilder builder = new TlvBuilder(0x145);
            builder.PushBytes(guid);
            return builder.GetPacket();
        }

        public static byte[] T147(long appId, string apkVersionName, byte[] apkSignatureMd5)
        {
            TlvBuilder builder = new TlvBuilder(0x147);
            builder.PushInt32((int)appId);
            builder.PushString(apkVersionName, true, true, 32);
            builder.PushBytes(apkSignatureMd5, false, true, true, 32);
            return builder.GetPacket();
        }

        public static byte[] T154(int ssoSequenceId)
        {
            TlvBuilder builder = new TlvBuilder(0x154);
            builder.PushInt32(ssoSequenceId);
            return builder.GetPacket();
        }

        public static byte[] T177(long buildTime = 1577331209, string sdkVersion = "6.0.0.2425")
        {
            TlvBuilder builder = new TlvBuilder(0x177);
            builder.PushInt8(1);
            builder.PushInt32((int)buildTime);
            builder.PushString(sdkVersion);
            return builder.GetPacket();
        }

        public static byte[] T187(byte[] macAddress)
        {
            TlvBuilder builder = new TlvBuilder(0x187);
            builder.PushBytes(Md5.Create(macAddress));
            return builder.GetPacket();
        }

        public static byte[] T188(byte[] androidId)
        {
            TlvBuilder builder = new TlvBuilder(0x188);
            builder.PushBytes(Md5.Create(androidId));
            return builder.GetPacket();
        }

        public static byte[] T191(int unknownK = 0x82)
        {
            TlvBuilder builder = new TlvBuilder(0x191);
            builder.PushUInt8((byte)unknownK);
            return builder.GetPacket();
        }

        public static byte[] T202(byte[] wifiBssid, string wifiSsid)
        {
            TlvBuilder builder = new TlvBuilder(0x202);
            builder.PushBytes(Md5.Create(wifiBssid), false, true, true, 16);
            builder.PushString(wifiSsid, true, true, 32);
            return builder.GetPacket();
        }

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

        public static byte[] T516(int sourceType = 0)
        {
            TlvBuilder builder = new TlvBuilder(0x516);
            builder.PushInt32(sourceType);
            return builder.GetPacket();
        }

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

        public static byte[] T536(byte[] loginExtraData)
        {
            TlvBuilder builder = new TlvBuilder(0x536);
            builder.PushBytes(loginExtraData, false);
            return builder.GetPacket();
        }

    }

}
