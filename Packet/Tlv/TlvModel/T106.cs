using System;
using Guid = Konata.Utils.Guid;

using Konata.Core;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T106Body : TlvBody
    {
        public readonly ushort _tgtgtVer;
        public readonly uint _ssoVer;
        public readonly uint _appId;
        public readonly uint _subAppId;
        public readonly uint _appClientVersion;
        public readonly uint _uin;
        public readonly string _uinString;
        public readonly byte[] _ipAddress;
        public readonly bool _isSavePassword;
        public readonly byte[] _passwordMd5;
        public readonly long _salt;
        public readonly byte[] _tgtgKey;
        public readonly bool _isGuidAvailable;
        public readonly byte[] _guid;
        public readonly LoginType _loginType;
        public readonly int _randomNumber;
        public readonly uint _timeNow;

        public T106Body(uint appId, uint subAppId, uint appClientVersion,
            uint uin, byte[] ipAddress, bool isSavePassword, byte[] passwordMd5, long salt,
            bool isGuidAvailable, byte[] guid, LoginType loginType, byte[] tgtgKey)
            : base()
        {
            _ssoVer = 6;
            _tgtgtVer = 4;

            _uin = uin;
            _uinString = uin.ToString();
            _salt = salt;
            _guid = guid;
            _appId = appId;
            _tgtgKey = tgtgKey;
            _loginType = loginType;
            _ipAddress = ipAddress;
            _subAppId = subAppId;
            _appClientVersion = appClientVersion;
            _isSavePassword = isSavePassword;
            _passwordMd5 = passwordMd5;
            _isGuidAvailable = isGuidAvailable;
            _randomNumber = new Random().Next();
            _timeNow = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            PutUshortBE(_tgtgtVer);
            PutIntBE(_randomNumber);
            PutUintBE(_ssoVer);
            PutUintBE(_appId);
            PutUintBE(_appClientVersion);
            PutLongBE(_uin == 0 ? _salt : _uin);
            PutUintBE(_timeNow);
            PutBytes(_ipAddress);
            PutBoolBE(_isSavePassword, 1);
            PutBytes(_passwordMd5, Prefix.None, 16);
            PutBytes(_tgtgKey, Prefix.None, 16);
            PutUintBE(0);
            PutBoolBE(_isGuidAvailable, 1);
            PutBytes(_isGuidAvailable ? _guid : Guid.Generate(), Prefix.None, 16);
            PutUintBE(_subAppId);
            PutUintBE((uint)_loginType);
            PutString(_uinString, Prefix.Uint16);
            PutUshortBE(0);
        }

        public T106Body(byte[] data)
            : base(data)
        {
            //TakeUshortBE(out _tgtgtVer);
            //TakeIntBE(out _randomNumber);
            //TakeUintBE(out _ssoVer);
            //TakeUintBE(out _appId);
            //TakeUintBE(out _appClientVersion);
            //TakeLongBE(out var uin); _salt = uin; _uin = (uint)uin;
            //TakeUintBE(out _timeNow);
            //TakeBytes(out _ipAddress, 4);
            //TakeBoolBE(out _isSavePassword, 1);
            //TakeBytes(out _passwordMd5, 16);
            //TakeBytes(out _tgtgKey, 16);
            //EatBytes(4);
            //TakeBoolBE(out _isGuidAvailable, 1);
            //TakeBytes(out _guid, 16);
            //TakeUintBE(out _subAppId);
            //// TakeUintBE(out var type); _loginType = (LoginType)type;
            //TakeString(out _uinString, Prefix.Uint16);
            //EatBytes(2);
        }
    }
}
