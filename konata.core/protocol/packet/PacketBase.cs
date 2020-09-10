using System;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Protocol.Packet
{
    public class PacketBase
    {

        public virtual byte[] GetBytes() => null;

        public byte[] GetEncryptedBytes(ICryptor cryptor, byte[] key = null) => cryptor.Encrypt(GetBytes(), key);

        public virtual bool TryParse(byte[] data) => false;

        public override string ToString() => Hex.Bytes2HexStr(GetBytes());

    }
}
