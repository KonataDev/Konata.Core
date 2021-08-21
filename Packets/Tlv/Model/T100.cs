using System;

namespace Konata.Core.Packets.Tlv.Model
{
    public class T100Body : TlvBody
    {
        public readonly uint _ssoVer;
        public readonly uint _sigmap;
        public readonly ushort _dbBufVer;

        public readonly uint _appId;
        public readonly uint _subAppId;
        public readonly uint _appClientVersion;

        public T100Body(uint appId, uint subAppId, uint appClientVersion)
        : base()
        {
            _appId = appId;
            _subAppId = subAppId;
            _appClientVersion = appClientVersion;

            _ssoVer = 6;
            _dbBufVer = 1;
            _sigmap = 34869472;

            PutUshortBE(_dbBufVer);
            PutUintBE(_ssoVer);
            PutUintBE(_appId);
            PutUintBE(_subAppId);
            PutUintBE(_appClientVersion);
            PutUintBE(_sigmap);
        }

        public T100Body(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _dbBufVer);
            TakeUintBE(out _ssoVer);
            TakeUintBE(out _appId);
            TakeUintBE(out _subAppId);
            TakeUintBE(out _appClientVersion);
            TakeUintBE(out _sigmap);
        }
    }
}
