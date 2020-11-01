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
            root3.AddLeafString("0A", "Hello Konata!");
            root2.AddTree("0A", root3);
            root1.AddTree("0A", root2);
            root.AddTree("0A", root1);

            Print(root.Serialize().GetBytes());

            return true;
        }
    }
}
