﻿using System;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x88d_0 : Test
    {
        public override bool Run()
        {
            var packet = new OidbCmd0x88d_0(2333);
            Print(packet);

            return true;
        }
    }
}