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
                PutJceTypeHeader(JceType.StructBegin, 0);
                {
                    Write(uin, 0);
                    Write(bid, 1);
                    Write(connType, 2);
                    Write(other, 3);
                    Write(status, 4);
                    Write(onlinePush, 5);
                    Write(isOnline, 6);
                    Write(isShowOnline, 7);
                    Write(kikPC, 8);
                    Write(kikWeak, 9);
                    Write(timeStamp, 10);
                    Write(osVersion, 11);
                    Write(netType, 12);

                    if (buildVer != null)
                    {
                        Write(buildVer, 13);
                    }

                    Write(regType, 14);

                    if (devParam != null)
                    {
                        Write(devParam, 15);
                    }

                    if (guid != null)
                    {
                        Write(guid, 16);
                    }

                    Write(localeID, 17);
                    Write(slientPush, 18);

                    if (devName != null)
                    {
                        Write(devName, 19);
                    }

                    if (devType != null)
                    {
                        Write(devType, 20);
                    }

                    if (osVer != null)
                    {
                        Write(osVer, 21);
                    }

                    Write(openPush, 22);
                    Write(largeSeq, 23);
                    Write(lastWatchStartTime, 24);

                    if (bindUin != null)
                    {
                        // Write(bindUin, 25);
                    }

                    Write(oldSSOIp, 26);
                    Write(newSSOIp, 27);

                    if (channelNo != null)
                    {
                        Write(channelNo, 28);
                    }

                    Write(cpId, 29);

                    if (vendorName != null)
                    {
                        Write(vendorName, 30);
                    }

                    if (vendorOSName != null)
                    {
                        Write(vendorOSName, 31);
                    }

                    if (osIdfa != null)
                    {
                        Write(osIdfa, 32);
                    }

                    if (cmd0x769Reqbody != null)
                    {
                        Write(cmd0x769Reqbody, 33);
                    }

                    Write(isSetStatus, 34);

                    if (serverBuf != null)
                    {
                        Write(serverBuf, 35);
                    }

                    Write(setMute, 36);
                    Write(extOnlineStatus, 38);
                    Write(batteryStatus, 39);
                }
                PutJceTypeHeader(JceType.StructEnd, 0);

                return this;
            }
        }
    }
}
