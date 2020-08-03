using Konata;
using Konata.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace konata
{
    class Konata
    {
        static void Main()
        {
            byte[] tlv = Tlv.T116(16252796, 66560, new long[] { 1600000226 });

            Console.WriteLine(Hex.Bytes2HexStr(tlv));
            Console.Read();
            return;
        }

    }
}
