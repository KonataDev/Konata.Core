using Konata.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Test
{
    public class BaseTest
    {
        public void Print_Bytes(params object[] args)
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
