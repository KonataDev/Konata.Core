using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Msf
{
    public class Packet
    {

        private byte[] _packetBuffer;
        private uint _packetLength;

        private void WriteBytes()
        {

        }

        public void PutSbyte(sbyte value)
        {

        }

        public void PutByte(byte value)
        {

        }

        public void PutShortBE(short value)
        {

        }
        public void PutUshortBE(ushort value)
        {

        }

        public void PutIntBE(int value)
        {

        }

        public void PutUintBE(uint value)
        {

        }

        public void PutLongBE(long value)
        {

        }

        public void PutUlongBE(ulong value)
        {

        }

        public void PutString(string value)
        {

        }

        public void PutBytes(byte[] value)
        {

        }

        public void PutPacket(Packet value)
        {
            PutBytes(value.GetBytes());
        }

        public void PutTlv(Packet value)
        {

        }

        public void FromPacket(Packet packet)
        {

        }

        public byte[] GetBytes()
        {
            return null;
        }
    }
}
