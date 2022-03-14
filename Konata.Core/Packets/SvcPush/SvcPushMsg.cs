using System.Linq;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Packets.SvcPush;

internal class SvcReqPushMsg : UniPacket
{
    public PushType EventType { get; private set; }

    public byte[] PushPayload { get; private set; }

    public int SvrIp { get; private set; }
    
    public uint FromSource { get; private set; }
    
    public int Unknown0x32 { get; private set; }
    
    public int Unknown0x1C { get; private set; }
    
    public byte[] Unknown0x8D { get; private set; }

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
                p.Unknown0x32 = vstruct["3"].Number.ValueInt;
                p.Unknown0x8D = vstruct["8"].SimpleList.Value;

                if (p.EventType == PushType.Friend)
                    p.FromSource = (uint) vstruct["0"].Number.Value;
                else if (p.EventType == PushType.Group)
                    p.FromSource = ByteConverter.BytesToUInt32(p.PushPayload.Take(4).ToArray(), 0, Endian.Big);
            }
            p.SvrIp = r["0.3"].Number.ValueInt;
        })
    {
    }
}

internal enum PushType
{
    Friend = 528,
    Group = 732
}
