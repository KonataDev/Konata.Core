using System;
using System.Linq;
using Konata.Utils;
using Konata.Utils.Crypt;
using Guid = Konata.Utils.Guid;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T106 : TlvBase
    {
        private const short _tgtgtVer = 4;
        private const int _ssoVer = 6;

        private readonly long _appId;
        private readonly long _subAppId;
        private readonly int _appClientVersion;
        private readonly long _uin;
        private readonly byte[] _ipAddress;
        private readonly bool _isSavePassword;
        private readonly byte[] _passwordMd5;
        private readonly long _salt;
        private readonly byte[] _tgtgKey;
        private readonly bool _isGuidAvailable;
        private readonly byte[] _guid;
        private readonly LoginType _loginType;

        public T106(long appId, long subAppId, int appClientVersion,
            long uin, byte[] ipAddress, bool isSavePassword, byte[] passwordMd5, long salt,
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
            builder.PushInt16(_tgtgtVer);
            builder.PushInt32(new Random().Next());
            builder.PushInt32(_ssoVer);
            builder.PushInt32((int)_appId);
            builder.PushInt32(_appClientVersion);
            builder.PushInt64(_uin == 0 ? _salt : _uin);
            builder.PushUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            builder.PushBytes(_ipAddress);
            builder.PushBool(_isSavePassword);
            builder.PushBytes(_passwordMd5, false);
            builder.PushBytes(_tgtgKey, false);
            builder.PushInt32(0);
            builder.PushBool(_isGuidAvailable);
            builder.PushBytes(_isGuidAvailable ? _guid : Guid.Generate(), false);
            builder.PushInt32((int)_subAppId);
            builder.PushInt32((int)_loginType);
            builder.PushString(_uin.ToString());
            builder.PushInt16(0);

            byte[] cryptKey = new Md5Cryptor().Encrypt(_passwordMd5.Concat(BitConverter.GetBytes(_uin).Reverse().ToArray()).ToArray());

            return builder.GetEncryptedBytes(new TeaCryptor(), cryptKey);
        }
    }
}
