using System;
using System.Linq;
using Konata.Crypto;
using konata.Utils;

namespace Konata.Utils
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
            byte[] tlvBody = GetStreamBytes();
            byte[] tlvLength = BitConverter.GetBytes((short)tlvBody.Length).Reverse().ToArray();

            return tlvCmd.Concat(tlvLength).Concat(tlvBody).ToArray();
        }

        public byte[] GetEnctyptedPacket(IKonataCryptor cryptor, byte[] cryptKey)
        {
            byte[] tlvCmd = BitConverter.GetBytes(cmd).Reverse().ToArray();
            byte[] tlvBody = cryptor.Encrypt(GetStreamBytes(), cryptKey);
            byte[] tlvLength = BitConverter.GetBytes((short)tlvBody.Length).Reverse().ToArray();

            return tlvCmd.Concat(tlvLength).Concat(tlvBody).ToArray();
        }

    }

}
