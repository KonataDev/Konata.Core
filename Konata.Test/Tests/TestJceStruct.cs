using System;
using System.Collections.Generic;
using Konata.Library.JceStruct;

namespace Konata.Test.Tests
{
    class TestJceStruct : Test
    {
        public override bool Run()
        {
            var root = new JceTreeRoot();
            {
                root.AddLeafNumber(1, 2);
                root.AddLeafNumber(2, 0);
                root.AddLeafNumber(3, 0);
                root.AddLeafNumber(4, 0);
                root.AddLeafString(5, "PushService");
                root.AddLeafString(6, "SvcReqRegister");

                root.AddTree(7, (JceTreeRoot root2) =>
                {
                    root2.AddLeafString(0, "Test");
                    root2.AddLeafString(1, "Test");
                    root2.AddLeafString(2, "Test");
                    root2.AddLeafString(3, "Test");
                    root2.AddLeafString(4, "Test");
                    root2.AddLeafString(5, "Test");
                    root2.AddLeafNumber(6, 2333);

                    root2.AddTree(7, (JceTreeRoot root3) =>
                    {
                        root3.AddLeafNumber(0, 2333);
                        root3.AddLeafMap(1, new Dictionary<string, int>
                        {
                            ["test"] = 233,
                            ["=w="] = 234,
                            ["dict_test"] = 235
                        });

                        root3.AddStruct(2, (JceTreeRoot s) =>
                        {
                            s.AddLeafNumber(0, 0);
                            s.AddLeafNumber(1, 0);
                            s.AddLeafNumber(2, 0);
                            s.AddLeafNumber(3, 0);
                        });
                    });
                });
            }

            var data = root.Serialize().GetBytes();
            Print(data);

            var deroot = new JceTreeRoot(data);
            {
                Print("[-]", deroot.GetLeafNumber(1, out var _).ToString());
                Print("   ", deroot.GetLeafNumber(2, out var _).ToString());
                Print("   ", deroot.GetLeafNumber(3, out var _).ToString());
                Print("   ", deroot.GetLeafNumber(4, out var _).ToString());
                Print("   ", deroot.GetLeafString(5, out var _));
                Print("   ", deroot.GetLeafString(6, out var _));

                deroot.GetTree(7, (JceTreeRoot deroot2) =>
                {
                    Print(" [-]", deroot2.GetLeafString(0, out var _));
                    Print("    ", deroot2.GetLeafString(1, out var _));
                    Print("    ", deroot2.GetLeafString(2, out var _));
                    Print("    ", deroot2.GetLeafString(3, out var _));
                    Print("    ", deroot2.GetLeafString(4, out var _));
                    Print("    ", deroot2.GetLeafString(5, out var _));
                    Print("    ", deroot2.GetLeafNumber(6, out var _).ToString());

                    //deroot2.GetTree(7, (JceTreeRoot deroot3) =>
                    //{
                    //    Print("  [-]", deroot3.GetLeafNumber(0, out var _).ToString());
                    //    // deroot3.GetLeafMap<Dictionary<string, int>>(1);

                    //    //root3.AddStruct(2, (JceTreeRoot s) =>
                    //    //{
                    //    //    s.AddLeafNumber(0, 0);
                    //    //    s.AddLeafNumber(1, 0);
                    //    //    s.AddLeafNumber(2, 0);
                    //    //    s.AddLeafNumber(3, 0);
                    //    //});
                    //});
                });
            }

            return true;
        }
    }
}
