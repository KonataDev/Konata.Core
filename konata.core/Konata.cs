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
            byte[] tlv = Tlv.T177();

            Console.WriteLine(Hex.Bytes2HexStr(tlv));
            Console.Read();
            return;
        }

    }
}
