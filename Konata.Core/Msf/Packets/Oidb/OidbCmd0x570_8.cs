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

            : base(1392, 8, (ByteBuffer t) =>
            {
                t.PutUintBE(group);
                t.PutHexString("20 00 01");
                t.PutUintBE(uin);
                t.PutUintBE(sec);
            })
        {

        }
    }
}
