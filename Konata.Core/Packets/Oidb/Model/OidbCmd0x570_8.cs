﻿using Konata.Core.Utils.IO;

namespace Konata.Core.Packets.Oidb.Model;

/// <summary>
/// 群禁言
/// </summary>
internal class OidbCmd0x570_8 : OidbSSOPkg
{
    public OidbCmd0x570_8(uint groupUin, uint memberUin, uint timeSeconds)
        : base(0x570, 0x08, 0x00, (ByteBuffer root) =>
        {
            root.PutUintBE(groupUin);
            root.PutHexString("20 00 01");
            root.PutUintBE(memberUin);
            root.PutUintBE(timeSeconds);
        })
    {
    }
}
