using System;
using Konata.Msf.Packets.Protobuf;

namespace Konata.Test.Tests
{
    class TestProtobufMsgWithDraw : Test
    {
        public override bool Run()
        {
            var packet = new ProtoMsgWithDraw(23333333, 1567772123);
            Print(packet.Serialize());

            return true;
        }
    }
}
