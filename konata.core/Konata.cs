using Konata;
using Konata.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata
{
    internal class Program
    {
        public static void Main()
        {
            byte[] tlv = Tlv.T147(16, "7.7.6", Hex.HexStr2Bytes("A6 B7 45 BF 24 A2 C2 77 52 77 16 F6 F3 6E B6 8D"));

            Console.WriteLine(Hex.Bytes2HexStr(tlv));
            Console.Read();
        }

    }
}
