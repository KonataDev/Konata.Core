using System;
using Konata;
using Konata.Utils;
using Konata.Protocol.Packet.Login;

namespace Konata.Debug
{
    internal class Program
    {
        public static void Main()
        {
            byte[] output = { };

            //byte[] key = Hex.HexStr2Bytes("01 55 C2 C0 CA A2 34 66 00 82 BC FF 33 80 EF FE");
            //byte[] data = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            //output = new TeaCryptor().Encrypt(data, key);

            output = Tlv.T144(
                Tlv.T109(Device.OS),
                Tlv.T52d(new byte[] { 1, 2, 3, 4, 5, 6 }),
                Tlv.T124(Device.OS, Device.OSVersion, 1, "中華電信", new byte[] { 0, 0 }, "ChinaNet"),
                Tlv.T128(false, true, false, new byte[] { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 }, 1, Device.Name, Device.Vendor),
                Tlv.T148("QQ", 0, 16, 0, "7.0.0", "sdsdf"),
                Tlv.T153(false),
                Tlv.T16e(Device.Name),
                Hex.HexStr2Bytes("12 B4 BB E7 CD E0 5F DA 91 78 A6 EE 2E 22 39 04"));

            Console.WriteLine(Hex.Bytes2HexStr(output));
            Console.Read();
        }
    }

}
