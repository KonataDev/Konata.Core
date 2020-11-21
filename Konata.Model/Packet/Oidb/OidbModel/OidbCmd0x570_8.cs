using System;
using Konata.Utils.IO;

namespace Konata.Model.Packet.Oidb.OidbModel
{
    /// <summary>
    /// 群禁言
    /// </summary>

    public class OidbCmd0x570_8 : OidbSSOPkg
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
}
