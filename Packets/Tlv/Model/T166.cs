namespace Konata.Core.Packets.Tlv.Model;

internal class T166Body : TlvBody
{
    public readonly byte _imgType;

    public T166Body(byte imgType)
        : base()
    {
        _imgType = imgType;

        PutByte(_imgType);
    }

    public T166Body(byte[] data)
        : base(data)
    {
        TakeByte(out _imgType);
    }
}
