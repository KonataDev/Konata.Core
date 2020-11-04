using System;
using System.Collections.Generic;
using Konata.Library.JceStruct;

namespace Konata.Test.Tests
{
    class TestJceStruct : Test
    {
        public override bool Run()
        {
            var root = new Jce.Struct
            {
                [1] = (Jce.Number)2,
                [2] = (Jce.Number)0,
                [3] = (Jce.Number)0,
                [4] = (Jce.Number)0,
                [5] = (Jce.String)"PushService",
                [6] = (Jce.String)"SvcReqRegister",

                [7] = (Jce.Struct)new Jce.Struct
                {
                    [0] = (Jce.String)"Test",
                    [1] = (Jce.String)"Test",
                    [2] = (Jce.String)"Test",
                    [3] = (Jce.String)"Test",
                    [4] = (Jce.String)"Test",
                    [5] = (Jce.String)"Test",
                    [6] = (Jce.Number)2333,

                    [7] = (Jce.Struct)new Jce.Struct
                    {
                        [0] = (Jce.Number)2333,

                        [1] = (Jce.Map)new Jce.Map
                        {
                            [(Jce.String)"test"] = (Jce.Number)233,
                            [(Jce.String)"=w="] = (Jce.Number)234,
                            [(Jce.String)"dict_test"] = (Jce.Number)235
                        },

                        [2] = (Jce.Struct)new Jce.Struct
                        {
                            [0] = (Jce.Number)0,
                            [1] = (Jce.Number)0,
                            [2] = (Jce.Number)0,
                            [3] = (Jce.Number)0,
                        },

                        [3] = (Jce.SimpleList)new Jce.SimpleList(new byte[]
                        {
                            0x00, 0x01, 0x02
                        })
                    }
                }
            };

            var data = Jce.Serialize(root);
            Print(data);

            //var deroot = new JceTreeRoot(data);
            //{
            //    Print("[-]", deroot.GetLeafNumber(1, out var _));
            //    Print("   ", deroot.GetLeafNumber(2, out var _));
            //    Print("   ", deroot.GetLeafNumber(3, out var _));
            //    Print("   ", deroot.GetLeafNumber(4, out var _));
            //    Print("   ", deroot.GetLeafString(5, out var _));
            //    Print("   ", deroot.GetLeafString(6, out var _));

            //    deroot.GetTree(7, (JceTreeRoot deroot2) =>
            //    {
            //        Print(" [-]", deroot2.GetLeafString(0, out var _));
            //        Print("    ", deroot2.GetLeafString(1, out var _));
            //        Print("    ", deroot2.GetLeafString(2, out var _));
            //        Print("    ", deroot2.GetLeafString(3, out var _));
            //        Print("    ", deroot2.GetLeafString(4, out var _));
            //        Print("    ", deroot2.GetLeafString(5, out var _));
            //        Print("    ", deroot2.GetLeafNumber(6, out var _));

            //        deroot2.GetTree(7, (JceTreeRoot deroot3) =>
            //        {
            //            Print("  [-]", deroot3.GetLeafNumber(0, out var _));

            //            Print("  [-] Map <string, ushort>");
            //            foreach (var element in deroot3.GetLeafMap<string, ushort>(1))
            //            {
            //                Print($"      [{element.Key}] => {element.Value}");
            //            }

            //            Print("  [-] Struct");
            //            deroot3.GetLeafStruct(2, (JceTreeRoot s) =>
            //            {
            //                Print("     ", s.GetLeafNumber(0, out var _));
            //                Print("     ", s.GetLeafNumber(1, out var _));
            //                Print("     ", s.GetLeafNumber(2, out var _));
            //                Print("     ", s.GetLeafNumber(3, out var _));
            //            });

            //            Print("     ", deroot3.GetLeafBytes(3, out var _));
            //        });
            //    });
            //}

            return true;
        }
    }
}
