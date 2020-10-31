using System;
using System.Collections;
using Konata.Library.JceStruct;
using Konata.Msf.Packets.Wup;

namespace Konata.Msf.Packets.SvcReq
{
    public struct RegisterInfo
    {
        public byte isOnline;
        public byte isSetStatus;
        public byte isShowOnline;
        public byte kikPC;
        public byte kikWeak;
        public byte onlinePush;
        public byte openPush;
        public byte regType;
        public byte setMute;
        public byte slientPush;
        public byte[] cmd0x769Reqbody;
        public byte connType;
        public byte netType;
        public int batteryStatus;
        public long largeSeq;
        public long lastWatchStartTime;
        public int localeID;
        public long osVersion;
        public int status;
        public long bid;
        public long cpId;
        public long uin;

        public string buildVer;
        public string channelNo;
        public string other;
        public string devName;
        public string devType;
        public string osIdfa;
        public string osVer;
        public string vendorName;
        public string vendorOSName;

        public long timeStamp;
        public long extOnlineStatus;
        public long newSSOIp;
        public long oldSSOIp;

        public ArrayList bindUin;
        public byte[] devParam;
        public byte[] guid;
        public byte[] serverBuf;
    }

    public class SvcReqRegister : UniPacket
    {
        //public SvcReqRegister(byte packetType, ushort messageType,
        //    ushort requestId, ushort oldRespIret, RegisterInfo info)

        //    : base("PushService", "SvcReqRegister", packetType, messageType,
        //          requestId, oldRespIret, new XSvcRegister(info))
        //{

        //}

        //public class XSvcRegister : UniPacketBodyV3
        //{
        //    public XSvcRegister(RegisterInfo info)
        //        : base("SvcReqRegister", new Func<JceTreeRoot>(() =>
        //        {
        //            return new JceTreeRoot()
        //            .AddStruct(0, (JceTreeRoot s) =>
        //            {
        //                s.AddLeafNumber(0, info.uin);
        //                s.AddLeafNumber(1, info.bid);
        //                s.AddLeafNumber(2, info.connType);
        //                s.AddLeafString(3, info.other);
        //                s.AddLeafNumber(4, info.status);
        //                s.AddLeafNumber(5, info.onlinePush);
        //                s.AddLeafNumber(6, info.isOnline);
        //                s.AddLeafNumber(7, info.isShowOnline);
        //                s.AddLeafNumber(8, info.kikPC);
        //                s.AddLeafNumber(9, info.kikWeak);
        //                s.AddLeafNumber(10, info.timeStamp);
        //                s.AddLeafNumber(11, info.osVersion);
        //                s.AddLeafNumber(12, info.netType);

        //                if (info.buildVer != null)
        //                    s.AddLeafString(13, info.buildVer);

        //                s.AddLeafNumber(14, info.regType);

        //                if (info.devParam != null)
        //                    s.AddLeafBytes(15, info.devParam);

        //                if (info.guid != null)
        //                    s.AddLeafBytes(16, info.guid);

        //                s.AddLeafNumber(17, info.localeID);
        //                s.AddLeafNumber(18, info.slientPush);

        //                if (info.devName != null)
        //                    s.AddLeafString(19, info.devName);

        //                if (info.devType != null)
        //                    s.AddLeafString(20, info.devType);

        //                if (info.osVer != null)
        //                    s.AddLeafString(21, info.osVer);

        //                s.AddLeafNumber(22, info.openPush);
        //                s.AddLeafNumber(23, info.largeSeq);
        //                s.AddLeafNumber(24, info.lastWatchStartTime);

        //                // if (info.bindUin != null)
        //                //    leaf.Write(25, info.bindUin);

        //                s.AddLeafNumber(26, info.oldSSOIp);
        //                s.AddLeafNumber(27, info.newSSOIp);

        //                if (info.channelNo != null)
        //                    s.AddLeafString(28, info.channelNo);

        //                s.AddLeafNumber(29, info.cpId);

        //                if (info.vendorName != null)
        //                    s.AddLeafString(30, info.vendorName);

        //                if (info.vendorOSName != null)
        //                    s.AddLeafString(31, info.vendorOSName);

        //                if (info.osIdfa != null)
        //                    s.AddLeafString(32, info.osIdfa);

        //                if (info.cmd0x769Reqbody != null)
        //                    s.AddLeafBytes(33, info.cmd0x769Reqbody);

        //                s.AddLeafNumber(34, info.isSetStatus);

        //                if (info.serverBuf != null)
        //                    s.AddLeafBytes(35, info.serverBuf);

        //                s.AddLeafNumber(36, info.setMute);
        //                s.AddLeafNumber(38, info.extOnlineStatus);
        //                s.AddLeafNumber(39, info.batteryStatus);
        //            });

        //        })())
        //    {

        //    }
        //}
    }
}
