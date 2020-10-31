using System;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x8fc_2 : Test
    {
        public override bool Run()
        {
            var packet = new OidbCmd0x8fc_2(2333, 23333, "咕咕");
            Print(packet);

            return true;
        }
    }
}
