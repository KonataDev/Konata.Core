using System;
using Konata.Utils.Jce;

namespace Konata.Test.Tests
{
    public class TestJceWriter : Test
    {
        public override bool Run()
        {
            var jce = new JceOutputStream();
            jce.Write(233, 1);
            jce.Write(233, 2);
            jce.Write(233, 3);
            jce.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4);

            Print(jce.ToString());

            return true;
        }
    }
}
