using System;
using Konata.Library.Protobuf;

namespace Konata.Test.Tests
{
    class TestProtobuf : Test
    {
        public override bool Run()
        {
            var root = new ProtoTreeRoot();
            var root1 = new ProtoTreeRoot();
            var root2 = new ProtoTreeRoot();
            var root3 = new ProtoTreeRoot();
            root3.addLeafString("0A", "Hello Konata!");
            root2.addTree("0A", root3);
            root1.addTree("0A", root2);
            root.addTree("0A", root1);

            Print(root.Serialize().GetBytes());

            return true;
        }
    }
}
