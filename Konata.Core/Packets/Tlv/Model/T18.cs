namespace Konata.Core.Packets.Tlv.Model;

internal class T18Body : TlvBody
{
    public readonly ushort _sigVer;
    public readonly ushort _pingVersion;
    public readonly ushort _alwaysZero;
    public readonly uint _ssoVersion;
    public readonly uint _appId;
    public readonly uint _appClientVersion;
    public readonly uint _uin;

    public T18Body(uint appId, uint appClientVersion, uint uin)
        : base()
    {
        _sigVer = 0;
        _pingVersion = 1;
        _alwaysZero = 0;
        _ssoVersion = 1536;
        _uin = uin;
        _appId = appId;
        _appClientVersion = appClientVersion;

        PutUshortBE(_pingVersion);
        PutUintBE(_ssoVersion);
        PutUintBE(_appId);
        PutUintBE(_appClientVersion);
        PutUintBE(_uin);
        PutUshortBE(_alwaysZero);
        PutUshortBE(0);
    }

    public T18Body(byte[] data)
        : base(data)
    {
        TakeUshortBE(out _pingVersion);
        TakeUintBE(out _ssoVersion);
        TakeUintBE(out _appId);
        TakeUintBE(out _appClientVersion);
        TakeUintBE(out _uin);
        TakeUshortBE(out _alwaysZero);
        EatBytes(2);
    }
}
