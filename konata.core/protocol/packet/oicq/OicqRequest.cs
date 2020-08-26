using System;
using System.Linq;
using System.Text;
using Konata.Utils;

namespace Konata.Protocol.Packet.Oicq
{
    public abstract class OicqRequest : PacketBase
    {
        protected short _cmd;
        protected short _subCmd;

        public override byte[] GetBytes() => null;

        public static bool TryParse(byte[] data, out OicqRequest request)
        {
            Console.WriteLine(Hex.Bytes2HexStr(data));

            request = new OicqRequestCheckImage();
            return true;
        }


    }
}
