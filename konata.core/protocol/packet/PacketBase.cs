using System;
using Konata.Utils;

namespace Konata.Protocol.Packet
{
    public class PacketBase
    {

        public virtual byte[] GetBytes() => null;

        public virtual void SetBytes(byte[] data) { }

        public byte[] ToBytes() => GetBytes();

        public override string ToString() => Hex.Bytes2HexStr(GetBytes());

    }
}
