using Konata.Core.Utils.IO;

namespace Konata.Core.Packets.Oidb.Model;

/// <summary>
/// 設置取消群管理員
/// </summary>
internal class OidbCmd0x55c_1 : OidbSSOPkg
{
    public OidbCmd0x55c_1(uint groupUin, uint memberUin, bool promoteAdmin)
        : base(0x55c, 0x01, null, (ByteBuffer root) =>
        {
            root.PutUintBE(groupUin);
            root.PutUintBE(memberUin);
            root.PutBoolBE(promoteAdmin, 1);
        })
    {
    }
}
