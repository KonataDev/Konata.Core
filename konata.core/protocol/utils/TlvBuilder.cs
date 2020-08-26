using System;
using System.Linq;
using Konata.Utils;
using Konata.Utils.Crypt;

namespace Konata.Protocol.Utils
{
    public class TlvBuilder : StreamBuilder
    {
        private readonly ushort cmd;

        public TlvBuilder(ushort tlvCmd)
        {
            cmd = tlvCmd;
        }

        public byte[] GetPacket()
        {
            byte[] tlvCmd = BitConverter.GetBytes(cmd).Reverse().ToArray();
            byte[] tlvBody = GetBytes();
            byte[] tlvLength = BitConverter.GetBytes((short)tlvBody.Length).Reverse().ToArray();

            return tlvCmd.Concat(tlvLength).Concat(tlvBody).ToArray();
        }

        public byte[] GetEnctyptedPacket(ICryptor cryptor, byte[] cryptKey)
        {
            byte[] tlvCmd = BitConverter.GetBytes(cmd).Reverse().ToArray();
            byte[] tlvBody = GetEncryptedBytes(cryptor, cryptKey);
            byte[] tlvLength = BitConverter.GetBytes((short)tlvBody.Length).Reverse().ToArray();

            return tlvCmd.Concat(tlvLength).Concat(tlvBody).ToArray();
        }

    }

}
