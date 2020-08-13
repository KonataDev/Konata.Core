using System;
using Konata.Utils;
using Konata.Utils.Crypto;

namespace Konata.Protocol.Packet
{
    public class PacketBase
    {

        public virtual byte[] GetBytes() => null;

        public byte[] GetEncryptedBytes(ICryptor cryptor, byte[] key = null) => cryptor.Encrypt(GetBytes(), key);

        public virtual bool SetBytes(byte[] data) => false;

        public override string ToString() => Hex.Bytes2HexStr(GetBytes());

    }
}
