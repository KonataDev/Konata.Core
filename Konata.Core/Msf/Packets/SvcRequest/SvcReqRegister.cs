using System;
using System.Collections;
using Konata.Msf.Packets.Wup;
using Konata.Library.JceStruct;

namespace Konata.Msf.Packets.SvcRequest
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
        public SvcReqRegister(RegisterInfo info)

            : base(0x03, "PushService", "SvcReqRegister",
                  "SvcReqRegister", 0x00, 0x00, 0x00,
                (out Jce.Struct w) => w = new Jce.Struct
                {
                    [0] = (Jce.Number)info.uin,
                    [1] = (Jce.Number)info.bid,
                    [2] = (Jce.Number)info.connType,
                    [3] = (Jce.String)info.other,
                    [4] = (Jce.Number)info.status,
                    [5] = (Jce.Number)info.onlinePush,
                    [6] = (Jce.Number)info.isOnline,
                    [7] = (Jce.Number)info.isShowOnline,
                    [8] = (Jce.Number)info.kikPC,
                    [9] = (Jce.Number)info.kikWeak,
                    [10] = (Jce.Number)info.timeStamp,
                    [11] = (Jce.Number)info.osVersion,
                    [12] = (Jce.Number)info.netType,

                    [13] = (Jce.String)info.buildVer,
                    [14] = (Jce.Number)info.regType,
                    [15] = (Jce.SimpleList)info.devParam,
                    [16] = (Jce.SimpleList)info.guid,
                    [17] = (Jce.Number)info.localeID,
                    [18] = (Jce.Number)info.slientPush,

                    [19] = (Jce.String)info.devName,
                    [20] = (Jce.String)info.devType,
                    [21] = (Jce.String)info.osVer,

                    [22] = (Jce.Number)info.openPush,
                    [23] = (Jce.Number)info.largeSeq,
                    [24] = (Jce.Number)info.lastWatchStartTime,

                    [26] = (Jce.Number)info.oldSSOIp,
                    [27] = (Jce.Number)info.newSSOIp,
                    [28] = (Jce.String)info.channelNo,
                    [29] = (Jce.Number)info.cpId,
                    [30] = (Jce.String)info.vendorName,

                    [31] = (Jce.String)info.vendorOSName,
                    [32] = (Jce.String)info.osIdfa,
                    [33] = (Jce.SimpleList)info.cmd0x769Reqbody,
                    [34] = (Jce.Number)info.isSetStatus,
                    [35] = (Jce.SimpleList)info.serverBuf,
                    [36] = (Jce.Number)info.setMute,
                    [38] = (Jce.Number)info.extOnlineStatus,
                    [39] = (Jce.Number)info.batteryStatus,
                })
        {

        }
    }
}
