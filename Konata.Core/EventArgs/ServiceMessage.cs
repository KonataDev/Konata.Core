using System;
using System.Text;
using System.Collections.Generic;

using Konata.Utils.IO;
using Konata.Model.Packet;
using Konata.Model.Packet.Sso;
using Konata.Runtime.Network;
using Konata.Runtime.Base.Event;

namespace Konata.Core.EventArgs
{
    public enum RequestFlag : byte
    {
        DefaultEmpty = 0x00,
        D2Authentication = 0x01,
        WtLoginExchange = 0x02,
    }

    public class ServiceMessage : KonataEventArgs
    {
        private string _headuin;
        private byte[] _payload;

        public string HeadUin
        {
            get => this._headuin;
        }

        public byte[] Payload
        {
            get => this._payload;
        }

        public RequestFlag MsgFlag { get; set; }

        public RequestPktType MsgPktType { get; set; }

        private ServiceMessage() { }

        public static bool ToServiceMessage(SocketPackage package, out ServiceMessage msg)
        {
            msg = new ServiceMessage();
            var data = new PacketBase(package.Data);

            data.TakeUintBE(out var pktType);
            {
                if (pktType != 0x0A && pktType != 0x0B)
                    return false;

                msg.MsgPktType = (RequestPktType)pktType;
            }

            data.TakeByte(out var reqFlag);
            {
                if (reqFlag != 0x00 && reqFlag != 0x01 && reqFlag != 0x02)
                    return false;
                msg.MsgFlag = (RequestFlag)reqFlag;
            }

            data.TakeByte(out var zeroByte);
            {
                if (zeroByte != 0x00)
                    return false;
            }

            data.TakeString(out var uin,
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            msg._headuin = uin;

            data.TakeAllBytes(out msg._payload);

            msg.Owner = package.Owner;

            return true;
        }
    }
}
