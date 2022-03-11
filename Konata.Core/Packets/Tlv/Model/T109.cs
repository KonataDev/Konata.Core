namespace Konata.Core.Packets.Tlv.Model;

internal class T109Body : TlvBody
{
    public readonly string _osType;

    public T109Body(string osType)
        : base()
    {
        _osType = osType;

        PutString(_osType);
    }

    public T109Body(byte[] data)
        : base(data)
    {
        TakeString(out _osType, Prefix.None);
    }
}
