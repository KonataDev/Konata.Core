using System;
using System.Linq;
using Konata.Utils;
using Konata.Library.IO;

namespace Konata.Test
{
    public class TestNotPassedException : Exception
    {
        public TestNotPassedException(string msg)
            : base(msg)
        {

        }
    }

    public abstract class Test
    {
        public Test() { }

        public abstract bool Run();

        public void Print(params object[] args)
        {
            Console.Write($"[ .... ] ");
            {
                foreach (var element in args)
                {
                    if (element.GetType() == typeof(byte[]))
                        Console.Write(Hex.Bytes2HexStr((byte[])element));
                    else
                        Console.Write(element.ToString());

                    Console.Write(" ");
                }
            }
            Console.Write("\n");
        }

        public void Assert(bool logic)
        {
            if (!logic)
                throw new TestNotPassedException("Assert fault.");
        }
    }
}
