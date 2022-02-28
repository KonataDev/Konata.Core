namespace Konata.Core.Packets.Tlv.Model;

internal class T401Body : TlvBody
{
    public readonly byte[] _gMd5;

    public T401Body(byte[] gMd5, object nil)
        : base()
    {
        _gMd5 = gMd5;

        PutBytes(_gMd5);
    }

    public T401Body(byte[] data)
        : base(data)
    {
        TakeBytes(out _gMd5, Prefix.None);
    }
}
