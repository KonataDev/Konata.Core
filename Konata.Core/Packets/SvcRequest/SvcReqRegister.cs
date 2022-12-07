using System.Collections;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.SvcRequest;

internal struct RegisterInfo
{
    public byte isOnline;
    public byte isSetStatus;
    public byte isShowOnline;
    public bool kikPC;
    public bool kikWeak;
    public byte onlinePush;
    public byte openPush;
    public byte regType;
    public byte setMute;
    public byte silentPush;
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

internal class SvcReqRegister : UniPacket
{
    public SvcReqRegister(RegisterInfo info) : base(0x03, "PushService", "SvcReqRegister",
        "SvcReqRegister", 0x00, 0x00, 0x00, (out JStruct w) => w = new JStruct
        {
            [0] = (JNumber) info.uin,
            [1] = (JNumber) info.bid,
            [2] = (JNumber) info.connType,
            [3] = (JString) info.other,
            [4] = (JNumber) info.status,
            [5] = (JNumber) info.onlinePush,
            [6] = (JNumber) info.isOnline,
            [7] = (JNumber) info.isShowOnline,
            [8] = (JNumber) (info.kikPC ? 1 : 0),
            [9] = (JNumber) (info.kikWeak ? 1 : 0),
            [10] = (JNumber) info.timeStamp,
            [11] = (JNumber) info.osVersion,
            [12] = (JNumber) info.netType,

            [13] = (JString) info.buildVer,
            [14] = (JNumber) info.regType,
            [15] = (JSimpleList) info.devParam,
            [16] = (JSimpleList) info.guid,
            [17] = (JNumber) info.localeID,
            [18] = (JNumber) info.silentPush,

            [19] = (JString) info.devName,
            [20] = (JString) info.devType,
            [21] = (JString) info.osVer,

            [22] = (JNumber) info.openPush,
            [23] = (JNumber) info.largeSeq,
            [24] = (JNumber) info.lastWatchStartTime,

            [26] = (JNumber) info.oldSSOIp,
            [27] = (JNumber) info.newSSOIp,
            [28] = (JString) info.channelNo,
            [29] = (JNumber) info.cpId,
            [30] = (JString) info.vendorName,

            [31] = (JString) info.vendorOSName,
            [32] = (JString) info.osIdfa,
            [33] = (JSimpleList) info.cmd0x769Reqbody,
            [34] = (JNumber) info.isSetStatus,
            [35] = (JSimpleList) info.serverBuf,
            [36] = (JNumber) info.setMute,
            [38] = (JNumber) info.extOnlineStatus,
            [39] = (JNumber) info.batteryStatus,
        })
    {
    }
}
