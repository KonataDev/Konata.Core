using System;
using System.Linq;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Packets.SvcPush
{
    public class SvcReqPushMsg : UniPacket
    {
        public PushType EventType;
        public byte[] PushPayload;

        public int SvrIp;
        public uint FromSource;
        public int Unknown0x32;
        public int Unknown0x1C;
        public byte[] Unknown0x8D;

        public SvcReqPushMsg(byte[] pushMsg)
            : base(pushMsg, (userdata, r) =>
            {
                var p = (SvcReqPushMsg) userdata;

                var list = (JList) r["0.2"];
                var vstruct = (JStruct) list["0"];
                {
                    // Parse type and data
                    p.EventType = (PushType) vstruct["2"].Number.ValueInt;
                    p.PushPayload = vstruct["6"].SimpleList.Value;

                    p.Unknown0x1C = vstruct["1"].Number.ValueInt;
                    p.SvrIp = vstruct["3"].Number.ValueInt;
                    p.Unknown0x8D = vstruct["8"].SimpleList.Value;

                    if (p.EventType == PushType.Friend)
                        p.FromSource = (uint) vstruct["0"].Number.Value;
                    else if (p.EventType == PushType.Group)
                        p.FromSource = ByteConverter.BytesToUInt32(p.PushPayload.Take(4).ToArray(), 0, Endian.Big);
                }
                p.Unknown0x32 = r["0.3"].Number.ValueInt;
            })
        {
        }
    }

    public enum PushType
    {
        Friend = 528,
        Group = 732
    }
}
