using System;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x88d_0 : Test
    {
        public override bool Run()
        {
            var packet = new OidbCmd0x88d_0(2333, new OidbCmd0x88d_0.GroupInfo
            {
                group_class_ext = 0,
                cmduin_join_time = 0,
                cmduin_join_msg_seq = 0,
                cmduin_join_real_msg_seq = 0
            });

            Print(packet);

            return true;
        }
    }
}
