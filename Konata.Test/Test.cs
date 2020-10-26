using Konata.Utils;
using System;

namespace Konata.Test
{
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
    }
}
