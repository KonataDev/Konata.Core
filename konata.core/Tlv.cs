using System;
using System.Collections.Generic;
using Konata.Utils;

namespace Konata
{
    static class Tlv
    {

        static public byte[] T001(ulong uin, byte[] IPAddress)
        {
            TlvBuilder builder = new TlvBuilder(0x01);
            builder.PushInt16(1); // _ip_ver
            builder.PushInt32(new Random().Next());
            builder.PushInt32((int)uin);
            builder.PushUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            builder.PushBytes(IPAddress);
            builder.PushInt16(0);
            return builder.GetPacket();
        }

        static public byte[] T008()
        {

            return new byte[0];
        }

        static public byte[] T018(long appID, int appClientVersion, ulong uin, int preservedBeZero)
        {
            TlvBuilder builder = new TlvBuilder(0x18);
            builder.PushInt16(1); // _ping_version
            builder.PushInt32(1536); // _sso_version
            builder.PushInt32((int)appID);
            builder.PushInt32(appClientVersion);
            builder.PushInt32((int)uin);
            builder.PushInt16((short)preservedBeZero);
            builder.PushInt16(0);
            return builder.GetPacket();
        }

        // 未完成 有加密
        static public byte[] T106(long appID, long subAppID, int appClientVersion,
            ulong uin, byte[] ipAddress, bool isSavePassword, byte[] passwordMD5, ulong salt,
            byte[] uinString, byte[] tgtgKey, bool isGUIDAvailable, byte[] guid, int loginType)
        {
            TlvBuilder builder = new TlvBuilder(0x106);
            builder.PushInt16(4); // _TGTGTVer
            builder.PushInt32(new Random().Next());
            builder.PushInt32(6); // _SSoVer
            builder.PushInt32((int)appID);
            builder.PushInt32(appClientVersion);
            builder.PushUInt64(uin == 0 ? salt : uin);
            //builder.PushInt8();
            builder.PushUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            //builder.PushInt8();
            builder.PushBytes(ipAddress);

            builder.PushInt32((int)subAppID);
            builder.PushInt32(loginType);
            builder.PushInt16(0);

            return builder.GetPacket();
        }

        static public byte[] T100(long appID, long subAppID, int appClientVersion)
        {
            TlvBuilder builder = new TlvBuilder(0x100);
            builder.PushInt16(1); // _db_buf_ver
            builder.PushInt32(6); // _sso_ver
            builder.PushInt32((int)appID);
            builder.PushInt32((int)subAppID);
            builder.PushInt32(appClientVersion);
            builder.PushInt32(34869472); // sigmap
            return builder.GetPacket();
        }



        static public byte[] T107(int picType, int capType = 0, int picSize = 0, int retType = 1)
        {
            TlvBuilder builder = new TlvBuilder(0x107);
            builder.PushInt16((short)picType);
            builder.PushInt8((sbyte)capType);
            builder.PushInt16((short)picSize);
            builder.PushInt8((sbyte)retType);
            return builder.GetPacket();
        }

        static public byte[] T108(byte[] ksid)
        {
            TlvBuilder builder = new TlvBuilder(0x108);
            builder.PushBytes(ksid);
            return builder.GetPacket();
        }

        static public byte[] T116(int i, int i2, long[] jArr)
        {

            return new byte[0];
        }

        static public byte[] T142(string apkID)
        {
            TlvBuilder builder = new TlvBuilder(0x142);
            builder.PushInt16(0); // _version
            builder.PushInt16(20); // limit_len
            builder.PushString(apkID);
            return builder.GetPacket();
        }

        static public byte[] T141()
        {

            return new byte[0];
        }

        // 未完成 有加密
        static public byte[] T144()
        {

            return new byte[0];
        }

        static public byte[] T145()
        {

            return new byte[0];
        }

        static public byte[] T147()
        {

            return new byte[0];
        }

        static public byte[] T154()
        {

            return new byte[0];
        }
        static public byte[] T177()
        {

            return new byte[0];
        }

        static public byte[] T187()
        {

            return new byte[0];
        }

        static public byte[] T188()
        {

            return new byte[0];
        }

        static public byte[] T191()
        {

            return new byte[0];
        }

        static public byte[] T202()
        {

            return new byte[0];
        }

        static public byte[] T511(List<string> Domains)
        {
            TlvBuilder builder = new TlvBuilder(0x511);

            builder.PushUInt16((ushort)Domains.Count);

            foreach (string element in Domains)
            {
                builder.PushInt8(1);
                builder.PushString(element);
            }

            return builder.GetPacket();
        }

        static public byte[] T516()
        {

            return new byte[0];
        }

        static public byte[] T521()
        {

            return new byte[0];
        }

        static public byte[] T525()
        {

            return new byte[0];
        }

    }

}
