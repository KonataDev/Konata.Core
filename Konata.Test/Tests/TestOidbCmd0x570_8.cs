using System;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x570_8 : Test
    {
        public override bool Run()
        {
            var oidb = new OidbCmd0x570_8(2333333, 12345678, 600);
            Print(oidb);

            return true;
        }
    }
}
