using System;
using Konata.Utils;
using Konata.Crypto;

namespace Konata
{
    internal class Program
    {
        public static void Main()
        {
            byte[] tlv = Tlv.T525(Tlv.T536(Hex.HexStr2Bytes("01 00")));

            Console.WriteLine(Hex.Bytes2HexStr(tlv));
            Console.Read();
        }

    }
}
