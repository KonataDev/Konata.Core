using Konata.Core.Packets.Wup;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Packets.SvcRequest;

internal class SvcReqSharpVideoMsg : UniPacket
{
    public SvcReqSharpVideoMsg(uint selfUin, uint friendUin, ProtoTreeRoot content)
        : base(0x03, "SharpSvr", "c2s", "SharpVideoMsg", 0x00, 0x00, 117456266,
            (out JStruct w) =>
            {
                w = new JStruct();
                {
                    w[0] = (JNumber) 1; // Version
                    w[1] = (JNumber) 1; // Type
                    w[2] = (JNumber) selfUin; // From uin
                    w[3] = new JList {(JNumber) friendUin}; // To uin
                    w[4] = new JSimpleList(ProtoTreeRoot.Serialize(content).GetBytes()); // Video buff
                    w[5] = (JNumber) 0; // msg uid
                    w[6] = (JNumber) 0; // msg seq
                    w[7] = (JNumber) 0; // msg type
                    w[8] = (JNumber) 0; // msg time
                    w[9] = (JNumber) 0; // call type
                    w[10] = (JNumber) 0; // client type
                }
            })
    {
    }
}
