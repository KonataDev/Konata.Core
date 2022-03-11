namespace Konata.Core.Packets.Tlv.Model;

internal class T10aBody : TlvBody
{
    public readonly byte[] _tgtToken;

    public T10aBody(byte[] tgtToken, object nil)
        : base()
    {
        _tgtToken = tgtToken;

        PutBytes(_tgtToken);
    }

    public T10aBody(byte[] data)
        : base(data)
    {
        TakeBytes(out _tgtToken, Prefix.None);
    }
}
