using System;

using Konata.Utils.IO;
using Konata.Core.Packet;
using Konata.Core.Packet.Sso;
using Konata.Runtime.Network;
using Konata.Runtime.Base.Event;

namespace Konata.Core.EventArgs
{
    public enum AuthFlag : byte
    {
        DefaultlyNo = 0x00,
        D2Authentication = 0x01,
        WtLoginExchange = 0x02,
    }

    public class ServiceMessage : KonataEventArgs
    {
        private string _headuin;
        private byte[] _payload;
        private AuthFlag _authFlag;
        private RequestPktType _msgPktType;

        public string HeadUin { get => _headuin; }

        public byte[] Payload { get => _payload; }

        public AuthFlag AuthFlag { get => _authFlag; }

        public RequestPktType MessagePktType { get => _msgPktType; }

        private ServiceMessage() { }

        public static bool Parse(SocketPackage package, out ServiceMessage serviceMsg)
        {
            serviceMsg = new ServiceMessage();
            serviceMsg.Owner = package.Owner;

            var r = new PacketBase(package.Data);
            {
                r.TakeUintBE(out var pktType);
                {
                    if (pktType != 0x0A && pktType != 0x0B)
                        return false;

                    serviceMsg._msgPktType = (RequestPktType)pktType;
                }

                r.TakeByte(out var reqFlag);
                {
                    if (reqFlag != 0x00 && reqFlag != 0x01 && reqFlag != 0x02)
                        return false;
                    serviceMsg._authFlag = (AuthFlag)reqFlag;
                }

                r.TakeByte(out var zeroByte);
                {
                    if (zeroByte != 0x00)
                        return false;
                }

                r.TakeString(out serviceMsg._headuin,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                r.TakeAllBytes(out serviceMsg._payload);
            }

            return true;
        }

        public static bool Pack()
            => throw new NotImplementedException();
    }
}
