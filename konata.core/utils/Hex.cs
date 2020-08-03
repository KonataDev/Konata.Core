using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Utils
{
    class Hex
    {

        private static readonly char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        private static readonly byte[] emptyBytes = new byte[0];

        public static string Byte2HexStr(byte b)
        {
            char[] buf = new char[2];
            buf[1] = digits[b & 15];
            buf[0] = digits[((byte)(b >> 4)) & 15];
            return new string(buf);
        }

        public static string Bytes2HexStr(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }
            char[] buf = new char[(bytes.Length * 2)];
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                buf[(i * 2) + 1] = digits[b & 15];
                buf[(i * 2) + 0] = digits[((byte)(b >> 4)) & 15];
            }
            return new string(buf);
        }

        public static byte HexStr2Byte(string str)
        {
            if (str == null || str.Length != 1)
            {
                return 0;
            }
            return Char2Byte(str[0]);
        }

        public static byte Char2Byte(char ch)
        {
            if (ch >= '0' && ch <= '9')
            {
                return (byte)(ch - '0');
            }
            if (ch >= 'a' && ch <= 'f')
            {
                return (byte)((ch - 'a') + 10);
            }
            if (ch < 'A' || ch > 'F')
            {
                return 0;
            }
            return (byte)((ch - 'A') + 10);
        }

        public static byte[] HexStr2Bytes(string str)
        {
            str = str.Replace(" ", "");

            if (str == null || str == "")
            {
                return emptyBytes;
            }
            byte[] bytes = new byte[(str.Length / 2)];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)((Char2Byte(str[i * 2]) * 16) + Char2Byte(str[(i * 2) + 1]));
            }
            return bytes;
        }

    }
}
