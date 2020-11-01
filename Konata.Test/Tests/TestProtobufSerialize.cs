using System;
using Konata.Library.Protobuf;

namespace Konata.Test.Tests
{
    class TestProtobufSerialize : Test
    {
        public override bool Run()
        {
            var root = new ProtoTreeRoot();
            root.AddTree("4A", (ProtoTreeRoot test) =>
            {
                test.AddLeafVar("08", 2333);
                test.AddLeafVar("18", 233333);
                test.AddLeafString("22", "This is a test.");
                test.AddTree("0A", (ProtoTreeRoot gugu) =>
                {
                    gugu.AddLeafString("22", "咕咕咕");
                });
            });

            var bytes = root.Serialize().GetBytes();
            Print(bytes);


            var deroot = new ProtoTreeRoot(bytes);
            deroot.GetTree("4A", (ProtoTreeRoot test) =>
            {
                Print(test.GetLeafVar("08", out var _).ToString());
                Print(test.GetLeafVar("18", out var _).ToString());
                Print(test.GetLeafString("22", out var _));

                test.GetTree("0A", (ProtoTreeRoot gugu) =>
                {
                    Print(gugu.GetLeafString("22", out var _));
                });
            });

            var deroot2 = new ProtoTreeRoot(bytes, true);
            Print(deroot2.GetLeafVar("4A.08", out var _).ToString());
            Print(deroot2.GetLeafVar("4A.18", out var _).ToString());
            Print(deroot2.GetLeafString("4A.22", out var _));
            Print(deroot2.GetLeafString("4A.0A.22", out var _));

            return true;
        }
    }
}
