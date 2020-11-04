using System;
using System.Linq;
using Konata.Library.JceStruct;

namespace Konata.Test.Tests
{
    class TestJceStructSimpleList : Test
    {
        public override bool Run()
        {
            var bytes = new byte[]
                { 0x00, 0x01, 0x02, 0x03 };

            var root = new Jce.Struct
            {
                [0] = new Jce.SimpleList(bytes)
            };

            var data = Jce.Serialize(root);
            Print(data);

            var deroot = Jce.Deserialize(data);
            Assert(Enumerable.SequenceEqual(((Jce.SimpleList)deroot["0"]).Value, bytes));

            return true;
        }
    }
}
