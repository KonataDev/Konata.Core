namespace Konata.Core.Packets.Tlv.Model;

internal class T143Body : TlvBody
{
    public readonly byte[] _d2Token;

    public T143Body(byte[] d2Token, object nil)
        : base()
    {
        _d2Token = d2Token;

        PutBytes(_d2Token);
    }

    public T143Body(byte[] data)
        : base(data)
    {
        TakeBytes(out _d2Token, Prefix.None);
    }
}
