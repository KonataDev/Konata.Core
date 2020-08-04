using System;
using Konata;
using Konata.Utils;

namespace Konata.Debug
{
    internal class Program
    {
        public static void Main()
        {
            byte[] output = { };
            //byte[] key = Hex.HexStr2Bytes("01 55 C2 C0 CA A2 34 66 00 82 BC FF 33 80 EF FE");
            //byte[] data = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            //byte[] encrypted = new Tea().Encrypt(data, key);

            output = Tlv.T2("egijekg", new byte[] { 1, 2, 3, 4, 5, 56, 5, 5, 5 });

            Console.WriteLine(Hex.Bytes2HexStr(output));
            Console.Read();
        }
    }

}
