namespace Konata.Core.Packets.Tlv.Model;

internal class T128Body : TlvBody
{
    public readonly bool _isNewInstall;
    public readonly bool _isGuidAvailable;
    public readonly bool _isGuidChanged;
    public readonly byte[] _guid;
    public readonly uint _guidFlag;
    public readonly string _deviceModel;
    public readonly string _deviceBrand;

    public T128Body(bool isNewInstall, bool isGuidAvailable, bool isGuidChanged,
        byte[] guid, uint guidFlag, string deviceModel, string deviceBrand)
        : base()
    {
        _isNewInstall = isNewInstall;
        _isGuidAvailable = isGuidAvailable;
        _isGuidChanged = isGuidChanged;
        _guid = guid;
        _guidFlag = guidFlag;
        _deviceModel = deviceModel;
        _deviceBrand = deviceBrand;

        PutUshortBE(0);
        PutBoolBE(_isNewInstall, 1);
        PutBoolBE(_isGuidAvailable, 1);
        PutBoolBE(_isGuidChanged, 1);
        PutUintBE(_guidFlag);
        PutString(_deviceModel, Prefix.Uint16, 32);
        PutBytes(_guid, Prefix.Uint16, 16);
        PutString(_deviceBrand, Prefix.Uint16, 16);
    }

    public T128Body(byte[] data)
        : base(data)
    {
        EatBytes(2);
        TakeBoolBE(out _isNewInstall, 1);
        TakeBoolBE(out _isGuidAvailable, 1);
        TakeBoolBE(out _isGuidChanged, 1);
        TakeUintBE(out _guidFlag);
        TakeString(out _deviceModel, Prefix.Uint16);
        TakeBytes(out _guid, Prefix.Uint16);
        TakeString(out _deviceBrand, Prefix.Uint16);
    }
}
