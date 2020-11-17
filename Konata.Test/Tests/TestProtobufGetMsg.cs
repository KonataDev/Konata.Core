using System;
using Konata.Packets.Protobuf;
using Konata.Library.Protobuf;

namespace Konata.Test.Tests
{
    class TestProtobufGetMsg : Test
    {
        public override bool Run()
        {
            var request = new ProtoGetMsg(new byte[38]);
            Print(request);

            return true;
        }
    }
}
