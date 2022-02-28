namespace Konata.Core.Packets.Tlv.Model;

internal class T10eBody : TlvBody
{
    public readonly byte[] _stKey;

    public T10eBody(byte[] stKey, object nil)
        : base()
    {
        _stKey = stKey;

        PutBytes(_stKey);
    }

    public T10eBody(byte[] data)
        : base(data)
    {
        TakeBytes(out _stKey, Prefix.None);
    }
}
