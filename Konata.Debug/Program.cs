using System;
using Konata;
using Konata.Msf.Packets.Tlv;
using Konata.Utils;

namespace Konata.Debug
{
    internal class Program
    {
        public static void Main()
        {
            Bot bot = new Bot(2051118019, "12345678");
            bot.Login();

            // var output = new T18(AppInfo.appId, AppInfo.appClientVersion, 2051118019);
            // Console.WriteLine(output.ToHexString());
            Console.Read();
        }
    }
}
