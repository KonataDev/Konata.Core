using System;
using Konata.Library.Protobuf;

namespace Konata.Test.Tests
{
    class TestProtobufMatryoshka : Test
    {
        public override bool Run()
        {
            var root0 = new ProtoTreeRoot();
            root0.AddTree("0A", (ProtoTreeRoot root1) =>
            {
                root1.AddTree("0A", (ProtoTreeRoot root2) =>
                {
                    root2.AddTree("0A", (ProtoTreeRoot root3) =>
                    {
                        root3.AddTree("0A", (ProtoTreeRoot root4) =>
                        {
                            root4.AddTree("0A", (ProtoTreeRoot root5) =>
                            {
                                root5.AddTree("0A", (ProtoTreeRoot root6) =>
                                {
                                    root6.AddTree("0A", (ProtoTreeRoot root7) =>
                                    {
                                        root7.AddTree("0A", (ProtoTreeRoot root8) =>
                                        {
                                            root8.AddTree("0A", (ProtoTreeRoot root9) =>
                                            {
                                                root9.AddTree("0A", (ProtoTreeRoot root10) =>
                                                {
                                                    root10.AddLeafString("0A", "I'm a matryoshka!");
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

            Print(root0.Serialize().GetBytes());

            return true;
        }
    }
}
