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
            byte[] tlv = Tlv.T187(Device.MacAddress);

            Console.WriteLine(Hex.Bytes2HexStr(tlv));
            Console.Read();
        }

    }
}
