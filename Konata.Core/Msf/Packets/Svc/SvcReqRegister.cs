using System;
using System.Collections;
using Konata.Library.JceStruct;

namespace Konata.Msf.Packets.Svc
{
    public class SvcReqRegister : SvcReq
    {
        public SvcReqRegister(byte packetType, ushort messageType,
            ushort requestId, ushort oldRespIret, XSvcRegister body)

            : base("PushService", "SvcReqRegister", packetType, messageType,
                  requestId, oldRespIret, body)
        {

        }

        public class XSvcRegister : SvcReqBody
        {
            public byte isOnline = 0;
            public byte isSetStatus = 0;
            public byte isShowOnline = 0;
            public byte kikPC = 0;
            public byte kikWeak = 0;
            public byte onlinePush = 0;
            public byte openPush = 1;
            public byte regType = 0;
            public byte setMute = 0;
            public byte slientPush = 0;
            public byte[] cmd0x769Reqbody;
            public byte connType = 0;
            public byte netType = 0;
            public int batteryStatus = 0;
            public long largeSeq = 0;
            public long lastWatchStartTime = 0;
            public int localeID = 2052;
            public long osVersion = 0;
            public int status = 11;
            public long bid = 0;
            public long cpId = 0;
            public long uin = 0;

            public string buildVer = "";
            public string channelNo = "";
            public string other = "";
            public string devName = "";
            public string devType = "";
            public string osIdfa = "";
            public string osVer = "";
            public string vendorName = "";
            public string vendorOSName = "";

            public long timeStamp = 0;
            public long extOnlineStatus = 0;
            public long newSSOIp = 0;
            public long oldSSOIp = 0;

            public ArrayList bindUin = null;
            public byte[] devParam = null;
            public byte[] guid = null;
            public byte[] serverBuf = null;

            public XSvcRegister()
                : base()
            {

            }

            public XSvcRegister Encode()
            {
                AddStruct(0, (JceTreeRoot s) =>
                {
                    s.AddLeafNumber(0, uin);
                    s.AddLeafNumber(1, bid);
                    s.AddLeafNumber(2, connType);
                    s.AddLeafString(3, other);
                    s.AddLeafNumber(4, status);
                    s.AddLeafNumber(5, onlinePush);
                    s.AddLeafNumber(6, isOnline);
                    s.AddLeafNumber(7, isShowOnline);
                    s.AddLeafNumber(8, kikPC);
                    s.AddLeafNumber(9, kikWeak);
                    s.AddLeafNumber(10, timeStamp);
                    s.AddLeafNumber(11, osVersion);
                    s.AddLeafNumber(12, netType);

                    if (buildVer != null)
                        s.AddLeafString(13, buildVer);

                    s.AddLeafNumber(14, regType);

                    if (devParam != null)
                        s.AddLeafBytes(15, devParam);

                    if (guid != null)
                        s.AddLeafBytes(16, guid);

                    s.AddLeafNumber(17, localeID);
                    s.AddLeafNumber(18, slientPush);

                    if (devName != null)
                        s.AddLeafString(19, devName);

                    if (devType != null)
                        s.AddLeafString(20, devType);

                    if (osVer != null)
                        s.AddLeafString(21, osVer);

                    s.AddLeafNumber(22, openPush);
                    s.AddLeafNumber(23, largeSeq);
                    s.AddLeafNumber(24, lastWatchStartTime);

                    // if (bindUin != null)
                    //    leaf.Write(25, bindUin);

                    s.AddLeafNumber(26, oldSSOIp);
                    s.AddLeafNumber(27, newSSOIp);

                    if (channelNo != null)
                        s.AddLeafString(28, channelNo);

                    s.AddLeafNumber(29, cpId);

                    if (vendorName != null)
                        s.AddLeafString(30, vendorName);

                    if (vendorOSName != null)
                        s.AddLeafString(31, vendorOSName);

                    if (osIdfa != null)
                        s.AddLeafString(32, osIdfa);

                    if (cmd0x769Reqbody != null)
                        s.AddLeafBytes(33, cmd0x769Reqbody);

                    s.AddLeafNumber(34, isSetStatus);

                    if (serverBuf != null)
                        s.AddLeafBytes(35, serverBuf);

                    s.AddLeafNumber(36, setMute);
                    s.AddLeafNumber(38, extOnlineStatus);
                    s.AddLeafNumber(39, batteryStatus);

                });

                return this;
            }
        }
    }
}
