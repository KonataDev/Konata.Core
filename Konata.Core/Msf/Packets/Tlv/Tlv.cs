using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlv
{
    public class Tlv : Packet
    {
        public readonly ushort _tlvCommand;
        public readonly ushort _tlvBodyLength;
        public readonly TlvBody _tlvBody;

        public Tlv(ushort tlvCommand, TlvBody tlvBody)
            : base()
        {
            _tlvCommand = tlvCommand;
            _tlvBody = tlvBody;

            PutUshortBE(tlvCommand);
            EnterBarrier(2, Endian.Big);
            {
                PutPacket(tlvBody);
            }
            LeaveBarrier();
        }

        public Tlv(ushort tlvCommand, TlvBody tlvBody, byte[] cryptKey)
            : base()
        {
            _tlvCommand = tlvCommand;
            _tlvBody = tlvBody;

            PutUshortBE(tlvCommand);
            EnterBarrierEncrypted(2, Endian.Big, TeaCryptor.Instance, cryptKey);
            {
                PutPacket(tlvBody);
            }
            LeaveBarrier();
        }

        public Tlv(ushort tlvCommand, byte[] data) : base(null)
        {
            _tlvCommand = tlvCommand;
            _tlvBodyLength = (ushort)data.Length;
            _tlvBody = TlvBody.FromBuffer(tlvCommand, data);
        }
    }

    public class TlvBody : Packet
    {
        public static TlvBody FromBuffer(ushort tlvCommand, byte[] data)
        {
            switch (tlvCommand)
            {
            case 0x0001: return new T1Body(data);
            case 0x0002: return new T2Body(data);
            case 0x0008: return new T8Body(data);
            case 0x0018: return new T18Body(data);
            case 0x0100: return new T100Body(data);
            case 0x0104: return new T104Body(data);
            case 0x0106: return new T106Body(data);
            case 0x0107: return new T107Body(data);
            case 0x0108: return new T108Body(data);
            case 0x0109: return new T109Body(data);
            case 0x0116: return new T116Body(data);
            case 0x0124: return new T124Body(data);
            case 0x0128: return new T128Body(data);
            case 0x0141: return new T141Body(data);
            case 0x0142: return new T142Body(data);
            case 0x0144: return new T144Body(data);
            case 0x0145: return new T145Body(data);
            case 0x0146: return new T146Body(data);
            case 0x0147: return new T147Body(data);
            case 0x0148: return new T148Body(data);
            case 0x0153: return new T153Body(data);
            case 0x0154: return new T154Body(data);
            case 0x0166: return new T166Body(data);
            case 0x016e: return new T16eBody(data);
            case 0x0174: return new T174Body(data);
            case 0x0177: return new T177Body(data);
            case 0x0178: return new T178Body(data);
            case 0x017a: return new T17aBody(data);
            case 0x017d: return new T17dBody(data);
            case 0x017e: return new T17eBody(data);
            case 0x0187: return new T187Body(data);
            case 0x0188: return new T188Body(data);
            case 0x0191: return new T191Body(data);
            case 0x0192: return new T192Body(data);
            case 0x0193: return new T193Body(data);
            case 0x0194: return new T194Body(data);
            case 0x0202: return new T202Body(data);
            case 0x0204: return new T204Body(data);
            case 0x0318: return new T318Body(data);
            case 0x0508: return new T508Body(data);
            case 0x0511: return new T511Body(data);
            case 0x0516: return new T516Body(data);
            case 0x0521: return new T521Body(data);
            case 0x0525: return new T525Body(data);
            case 0x052d: return new T52dBody(data);
            case 0x0536: return new T536Body(data);
            case 0x0544: return new T544Body(data);
            case 0x0545: return new T545Body(data);
            default: return null;
            }
        }

        public TlvBody()
            : base()
        {

        }

        public TlvBody(byte[] data)
            : base(data)
        {

        }
    }
}
