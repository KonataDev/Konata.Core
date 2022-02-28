namespace Konata.Core.Packets.Tlv.Model;

internal class T148Body : TlvBody
{
    public readonly string _appName;
    public readonly uint _ssoVersion;
    public readonly uint _appId;
    public readonly uint _subAppId;
    public readonly string _appVersion;
    public readonly string _appSignature;

    public T148Body(string appName, uint ssoVersion, uint appId, uint subAppId,
        string appVersion, string appSignature)
        : base()
    {
        _appName = appName;
        _ssoVersion = ssoVersion;
        _appId = appId;
        _subAppId = subAppId;
        _appVersion = appVersion;
        _appSignature = appSignature;

        PutString(_appName, Prefix.Uint16, 32);
        PutUintBE(_ssoVersion);
        PutUintBE(_appId);
        PutUintBE(_subAppId);
        PutString(_appVersion, Prefix.Uint16, 32);
        PutString(_appSignature, Prefix.Uint16, 32);
    }

    public T148Body(byte[] data)
        : base(data)
    {
        TakeString(out _appName, Prefix.Uint16);
        TakeUintBE(out _ssoVersion);
        TakeUintBE(out _appId);
        TakeUintBE(out _subAppId);
        TakeString(out _appVersion, Prefix.Uint16);
        TakeString(out _appSignature, Prefix.Uint16);
    }
}
