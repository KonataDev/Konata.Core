using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Oidb.Model;

/// <summary>
/// 戳一戳群員
/// </summary>
internal class OidbCmd0xed3 : OidbSSOPkg
{
    public OidbCmd0xed3(uint groupUin, uint memberUin)
        : base(0xed3, 0x01, null, (ByteBuffer root) =>
        {
            root.PutBytes(Proto.Encode(new Proto.Tree
            {
                {1, memberUin},
                {2, groupUin}
            }));
        })
    {
    }
}
