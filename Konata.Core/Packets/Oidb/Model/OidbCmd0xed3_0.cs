namespace Konata.Core.Packets.Oidb.Model;

internal class OidbCmd0xed3_0 : OidbCmd0xed3
{
    public OidbCmd0xed3_0(uint groupUin, uint memberUin)
        : base(0x01, new ReqBody
        {
            to_uin = memberUin,
            group_code = groupUin
        })
    {
    }

    public OidbCmd0xed3_0(uint selfUin, uint friendUin, bool flag)
        : base(0x01, new ReqBody
        {
            to_uin = friendUin,
            aio_uin = selfUin
        })
    {
    }
}
