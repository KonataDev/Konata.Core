using System;
using System.Linq;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;
using Guid = Konata.Utils.Guid;

namespace Konata.Msf.Packets.Tlvs
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
            byte[] tgtgKey, bool isGuidAvailable, byte[] guid, LoginType loginType)
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
        }

        public override ushort GetTlvCmd()
        {
            return 0x106;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(_tgtgtVer);
            builder.PutUintBE((uint)new Random().Next());
            builder.PutUintBE(_ssoVer);
            builder.PutUintBE(_appId);
            builder.PutUintBE(_appClientVersion);
            builder.PutLongBE(_uin == 0 ? _salt : _uin);
            builder.PutUintBE((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            builder.PutBytes(_ipAddress);
            builder.PutBoolBE(_isSavePassword, 1);
            builder.PutBytes(_passwordMd5);
            builder.PutBytes(_tgtgKey);
            builder.PutUintBE(0);
            builder.PutBoolBE(_isGuidAvailable, 2);
            builder.PutBytes(_isGuidAvailable ? _guid : Guid.Generate());
            builder.PutUintBE(_subAppId);
            builder.PutUintBE((uint)_loginType);
            builder.PutString(_uin.ToString());
            builder.PutUshortBE(0);

            byte[] cryptKey = new Md5Cryptor().Encrypt(_passwordMd5.Concat(BitConverter.GetBytes(_uin).Reverse().ToArray()).ToArray());

            return builder.GetEncryptedBytes(new TeaCryptor(), cryptKey);
        }
    }
}
