namespace Konata.Core.Packets.Tlv.Model;

internal class T16eBody : TlvBody
{
    public readonly string _deviceName;

    public T16eBody(string deviceName)
        : base()
    {
        _deviceName = deviceName;

        PutString(_deviceName);
    }

    public T16eBody(byte[] data)
        : base(data)
    {
        TakeString(out _deviceName, Prefix.None);
    }
}
