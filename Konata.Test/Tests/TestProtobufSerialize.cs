using System;
using Konata.Library.Protobuf;

namespace Konata.Test.Tests
{
    class TestProtobufSerialize : Test
    {
        public override bool Run()
        {
            var root = new ProtoTreeRoot();
            root.addTree("4A", (ProtoTreeRoot test) =>
            {
                test.addLeafVar("08", 2333);
                test.addLeafVar("18", 233333);
                test.addLeafString("22", "This is a test.");
                test.addTree("0A", (ProtoTreeRoot gugu) =>
                {
                    gugu.addLeafString("22", "咕咕咕");
                });
            });

            var bytes = root.Serialize().GetBytes();
            Print(bytes);


            var deroot = new ProtoTreeRoot(bytes);
            deroot.getTree("4A", (ProtoTreeRoot test) =>
            {
                Print(test.getLeafVar("08", out var _).ToString());
                Print(test.getLeafVar("18", out var _).ToString());
                Print(test.getLeafString("22", out var _));

                test.getTree("0A", (ProtoTreeRoot gugu) =>
                {
                    Print(gugu.getLeafString("22", out var _));
                });
            });

            var deroot2 = new ProtoTreeRoot(bytes, true);
            Print(deroot2.getLeafVar("4A.08", out var _).ToString());
            Print(deroot2.getLeafVar("4A.18", out var _).ToString());
            Print(deroot2.getLeafString("4A.22", out var _));
            Print(deroot2.getLeafString("4A.0A.22", out var _));

            return true;
        }
    }
}
