using System;
using System.Collections.Generic;
using System.IO;
using Konata.Crypto;

namespace Konata.Packets.Tlv
{
    using TlvMap = Dictionary<ushort, Tlv>;

    public class TlvUnpacker
    {
        private readonly TlvMap map = new TlvMap();

        public TlvUnpacker(byte[] data, bool prefixTlvCount = false)
        {
            PacketBase packet = new PacketBase(data);
            int count = 0;
            int countMax = 0;
            if (prefixTlvCount)
            {
                countMax = packet.TakeUshortBE(out ushort _);
            }
            while (packet.RemainLength > 0)
            {
                ++count;
                packet.TakeTlvData(out byte[] tlv, out ushort cmd);
                if (map.ContainsKey(cmd))
                {
                    map[cmd] = new Tlv(cmd, tlv);
                    // throw new IOException("Repeated Tlv command.");
                }
                else
                {
                    map.Add(cmd, new Tlv(cmd, tlv));
                }
            }
            if (prefixTlvCount && count < countMax)
            {
                throw new IOException("Insufficient Tlv number.");
            }
        }

        //  00 02 // prefixTlvCount

        //  01 04 // tlvCommand
        //  00 24 // Length:36
        //  41 75 33 54 47

        //  01 92 // tlvCommand
        //  00 CE // Length:206
        //  68 74 74 70 73 3A 2F 65 73

        public Tlv TryGetTlv(ushort tlvCommand)
        {
            return map.ContainsKey(tlvCommand) ? map[tlvCommand] : null;
        }

        public int Count { get { return map.Count; } }
    }
}
