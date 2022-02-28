namespace Konata.Core.Packets.Tlv.Model;

internal class T179Body : TlvBody
{
    public readonly ushort _verifyUrlLen;

    public T179Body(ushort verifyUrlLen)
        : base()
    {
        _verifyUrlLen = verifyUrlLen;

        PutUshortBE(_verifyUrlLen);
    }

    public T179Body(byte[] data)
        : base(data)
    {
        TakeUshortBE(out _verifyUrlLen);
    }
}
