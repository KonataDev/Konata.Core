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
            byte[] tlv = Tlv.T511(
                new string[] {
                    "tenpay.com",
                    "openmobile.qq.com",
                    "docs.qq.com",
                    "connect.qq.com",
                    "qzone.qq.com",
                    "vip.qq.com",
                    "qun.qq.com",
                    "game.qq.com",
                    "qqweb.qq.com",
                    "office.qq.com",
                    "ti.qq.com",
                    "mail.qq.com",
                    "qzone.com",
                    "mma.qq.com"
                });

            Console.WriteLine(Hex.Bytes2HexStr(tlv));
            Console.Read();
            return;
        }

    }
}
