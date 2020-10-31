using System;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x8a0_1 : Test
    {
        public override bool Run()
        {
            var packet = new OidbCmd0x8a0_1(23333, 2333, true);
            Print(packet);

            return true;
        }
    }
}
