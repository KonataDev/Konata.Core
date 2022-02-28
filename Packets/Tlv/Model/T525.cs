namespace Konata.Core.Packets.Tlv.Model;

internal class T525Body : TlvBody
{
    public readonly Tlv _t536;

    public T525Body(Tlv t536)
        : base()
    {
        _t536 = t536;

        PutUshortBE(1);
        PutTlv(_t536);
    }

    public T525Body(byte[] data)
        : base(data)
    {
        EatBytes(1);

        TakeTlvData(out var tlv, out var cmd); // cmd should be 0x0536
        _t536 = new Tlv(cmd, tlv);
    }
}
