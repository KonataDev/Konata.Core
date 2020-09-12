using System;
using System.Threading.Tasks;
using Konata;
using Konata.Utils;

namespace Konata.Debug
{
    internal class Program
    {
        public static void Main()
        {
            byte[] output = { };

            Bot bot = new Bot(2051118019, "12345678");
            var _ = bot.Login();

            Console.Read();
        }
    }

}
