using System;
using Konata.Library.Protobuf;

namespace Konata.Test.Tests
{
    class TestProtobufMatryoshka : Test
    {
        public override bool Run()
        {
            var root0 = new ProtoTreeRoot();
            root0.addTree("0A", (ProtoTreeRoot root1) =>
            {
                root1.addTree("0A", (ProtoTreeRoot root2) =>
                {
                    root2.addTree("0A", (ProtoTreeRoot root3) =>
                    {
                        root3.addTree("0A", (ProtoTreeRoot root4) =>
                        {
                            root4.addTree("0A", (ProtoTreeRoot root5) =>
                            {
                                root5.addTree("0A", (ProtoTreeRoot root6) =>
                                {
                                    root6.addTree("0A", (ProtoTreeRoot root7) =>
                                    {
                                        root7.addTree("0A", (ProtoTreeRoot root8) =>
                                        {
                                            root8.addTree("0A", (ProtoTreeRoot root9) =>
                                            {
                                                root9.addTree("0A", (ProtoTreeRoot root10) =>
                                                {
                                                    root10.addLeafString("0A", "I'm a matryoshka!");
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });

            Print(ProtoSerializer.Serialize(root0).GetBytes());

            return true;
        }
    }
}
