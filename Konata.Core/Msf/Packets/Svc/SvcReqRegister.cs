using System;
using System.Collections;
using Konata.Utils.Jce;

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
            public byte _isOnline = 0;
            public byte _isSetStatus = 0;
            public byte _isShowOnline = 0;
            public byte _kikPC = 0;
            public byte _kikWeak = 0;
            public byte _onlinePush = 0;
            public byte _openPush = 1;
            public byte _regType = 0;
            public byte _setMute = 0;
            public byte _slientPush = 0;
            public byte[] _cmd0x769Reqbody;
            public byte _connType = 0;
            public byte _netType = 0;
            public int _batteryStatus = 0;
            public long _largeSeq = 0;
            public long _lastWatchStartTime = 0;
            public int _localeID = 2052;
            public long _osVersion = 0;
            public int _status = 11;
            public long _bid = 0;
            public long _cpId = 0;
            public long _uin = 0;

            public string _buildVer = "";
            public string _channelNo = "";
            public string _other = "";
            public string _devName = "";
            public string _devType = "";
            public string _osIdfa = "";
            public string _osVer = "";
            public string _vendorName = "";
            public string _vendorOSName = "";

            public long _timeStamp = 0;
            public long _extOnlineStatus = 0;
            public long _newSSOIp = 0;
            public long _oldSSOIp = 0;

            public ArrayList _bindUin = null;
            public byte[] _devParam = null;
            public byte[] _guid = null;
            public byte[] _serverBuf = null;

            public XSvcRegister()
                : base()
            {

            }

            public XSvcRegister Encode()
            {
                PutJceTypeHeader(JceType.StructBegin, 0);
                {
                    Write(_uin, 0);
                    Write(_bid, 1);
                    Write(_connType, 2);
                    Write(_other, 3);
                    Write(_status, 4);
                    Write(_onlinePush, 5);
                    Write(_isOnline, 6);
                    Write(_isShowOnline, 7);
                    Write(_kikPC, 8);
                    Write(_kikWeak, 9);
                    Write(_timeStamp, 10);
                    Write(_osVersion, 11);
                    Write(_netType, 12);

                    if (_buildVer != null)
                    {
                        Write(_buildVer, 13);
                    }

                    Write(_regType, 14);

                    if (_devParam != null)
                    {
                        Write(_devParam, 15);
                    }

                    if (_guid != null)
                    {
                        Write(_guid, 16);
                    }

                    Write(_localeID, 17);
                    Write(_slientPush, 18);

                    if (_devName != null)
                    {
                        Write(_devName, 19);
                    }

                    if (_devType != null)
                    {
                        Write(_devType, 20);
                    }

                    if (_osVer != null)
                    {
                        Write(_osVer, 21);
                    }

                    Write(_openPush, 22);
                    Write(_largeSeq, 23);
                    Write(_lastWatchStartTime, 24);

                    if (_bindUin != null)
                    {
                        // Write(_bindUin, 25);
                    }

                    Write(_oldSSOIp, 26);
                    Write(_newSSOIp, 27);

                    if (_channelNo != null)
                    {
                        Write(_channelNo, 28);
                    }

                    Write(_cpId, 29);

                    if (_vendorName != null)
                    {
                        Write(_vendorName, 30);
                    }

                    if (_vendorOSName != null)
                    {
                        Write(_vendorOSName, 31);
                    }

                    if (_osIdfa != null)
                    {
                        Write(_osIdfa, 32);
                    }

                    if (_cmd0x769Reqbody != null)
                    {
                        Write(_cmd0x769Reqbody, 33);
                    }

                    Write(_isSetStatus, 34);

                    if (_serverBuf != null)
                    {
                        Write(_serverBuf, 35);
                    }

                    Write(_setMute, 36);
                    Write(_extOnlineStatus, 38);
                    Write(_batteryStatus, 39);
                }
                PutJceTypeHeader(JceType.StructEnd, 0);

                return this;
            }
        }
    }
}
