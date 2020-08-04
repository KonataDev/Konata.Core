using System;
using Konata.Utils;
using Konata.Crypto;

namespace Konata
{
    internal class Program
    {
        public static void Main()
        {
            byte[] tlv = Tlv.T124("android", "9.0.0",
                0, "中華電信", new byte[] { 0x00 }, "wifi");

            Console.WriteLine(Hex.Bytes2HexStr(tlv));
            Console.Read();
        }

    }
}
