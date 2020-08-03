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
            byte[] tlv = Tlv.T525(Tlv.T536(Hex.HexStr2Bytes("01 00")));

            Console.WriteLine(Hex.Bytes2HexStr(tlv));
            Console.Read();
        }

    }
}
