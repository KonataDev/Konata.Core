using System;
using Konata.Library.IO;

namespace Konata.Msf.Packets.Oidb
{
    /// <summary>
    /// 群禁言
    /// </summary>

    public class OidbCmd0x570_8 : OidbSSOPkg
    {
        public OidbCmd0x570_8(uint group, uint uin, uint sec)

            : base(0x570, 0x08, (ByteBuffer root) =>
            {
                root.PutUintBE(group);
                root.PutHexString("20 00 01");
                root.PutUintBE(uin);
                root.PutUintBE(sec);
            })
        {

        }
    }
}
