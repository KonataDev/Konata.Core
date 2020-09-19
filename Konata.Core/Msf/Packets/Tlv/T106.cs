using System;
using System.Linq;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;
using Guid = Konata.Utils.Guid;

namespace Konata.Msf.Packets.Tlv
{
    public class T106 : TlvBase
    {
        private const ushort _tgtgtVer = 4;
        private const uint _ssoVer = 6;

        private readonly uint _appId;
        private readonly uint _subAppId;
        private readonly uint _appClientVersion;
        private readonly uint _uin;
        private readonly byte[] _ipAddress;
        private readonly bool _isSavePassword;
        private readonly byte[] _passwordMd5;
        private readonly long _salt;
        private readonly byte[] _tgtgKey;
        private readonly bool _isGuidAvailable;
        private readonly byte[] _guid;
        private readonly LoginType _loginType;

        public T106(uint appId, uint subAppId, uint appClientVersion,
            uint uin, byte[] ipAddress, bool isSavePassword, byte[] passwordMd5, long salt,
            byte[] tgtgKey, bool isGuidAvailable, byte[] guid, LoginType loginType) : base()
        {
            _appId = appId;
            _subAppId = subAppId;
            _appClientVersion = appClientVersion;
            _uin = uin;
            _ipAddress = ipAddress;
            _isSavePassword = isSavePassword;
            _passwordMd5 = passwordMd5;
            _salt = salt;
            _tgtgKey = tgtgKey;
            _isGuidAvailable = isGuidAvailable;
            _guid = guid;
            _loginType = loginType;

            PackEncrypted(TeaCryptor.Instance, GetCryptKey());
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x0106);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(_tgtgtVer);
            PutUintBE((uint)new Random().Next());
            PutUintBE(_ssoVer);
            PutUintBE(_appId);
            PutUintBE(_appClientVersion);
            PutLongBE(_uin == 0 ? _salt : _uin);
            PutUintBE((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            PutBytes(_ipAddress);
            PutBoolBE(_isSavePassword, 1);
            PutBytes(_passwordMd5);
            PutBytes(_tgtgKey);
            PutUintBE(0);
            PutBoolBE(_isGuidAvailable, 1);
            PutBytes(_isGuidAvailable ? _guid : Guid.Generate());
            PutUintBE(_subAppId);
            PutUintBE((uint)_loginType);
            PutString(_uin.ToString(), 2);
            PutUshortBE(0);
        }

        private byte[] GetCryptKey()
        {
            return new Md5Cryptor().Encrypt(_passwordMd5
                .Concat(new byte[] { 0x00, 0x00, 0x00, 0x00 })
                .Concat(BitConverter.GetBytes(_uin).Reverse()).ToArray());
        }
    }
}
