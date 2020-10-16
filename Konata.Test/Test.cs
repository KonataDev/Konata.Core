using Konata.Utils;
using System;

namespace Konata.Test
{
    public abstract class Test
    {
        public Test() { }

        public abstract bool Run();

        public void Print(params string[] args)
        {
            Console.WriteLine($"[ .... ] {string.Join(" ", args)}");
        }

        public void Print(byte[] arg)
        {
            Print(Hex.Bytes2HexStr(arg));
        }
    }
}
