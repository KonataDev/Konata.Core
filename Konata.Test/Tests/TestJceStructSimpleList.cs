using Konata.Library.JceStruct;
using System;

namespace Konata.Test.Tests
{
    class TestJceStructSimpleList : Test
    {
        public override bool Run()
        {
            var root = new Jce.Struct
            {
                [0] = new Jce.SimpleList(new byte[]
                {
                    0x00, 0x01, 0x02, 0x03
                })
            };

            Print(Jce.Serialize(root));

            return true;
        }
    }
}
