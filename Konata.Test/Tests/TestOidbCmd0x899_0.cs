using System;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x899_0 : Test
    {
        public override bool Run()
        {
            var oidb = new OidbCmd0x899_0(233333, 0, 2, new OidbCmd0x899_0.MemberList
            {
                member_uin = 0
            });

            Print(oidb);

            return true;
        }
    }
}
