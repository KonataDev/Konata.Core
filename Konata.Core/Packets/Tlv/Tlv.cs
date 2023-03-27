using Konata.Core.Utils.IO;
using Konata.Core.Utils.Crypto;
using Konata.Core.Packets.Tlv.Model;

namespace Konata.Core.Packets.Tlv;

internal class Tlv : PacketBase
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
        EnterBarrier(Prefix.Uint16, Endian.Big);
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
        EnterBarrierEncrypted(Prefix.Uint16, Endian.Big, TeaCryptor.Instance, cryptKey);
        {
            PutPacket(tlvBody);
        }
        LeaveBarrier();
    }

    public Tlv(ushort tlvCommand, byte[] data) : base(null)
    {
        _tlvCommand = tlvCommand;
        _tlvBodyLength = (ushort) data.Length;
        _tlvBody = TlvBody.FromBuffer(tlvCommand, data);
    }
}

internal class TlvBody : PacketBase
{
    public static TlvBody FromBuffer(ushort tlvCommand, byte[] data)
    {
        return tlvCommand switch
        {
            0x0001 => new T1Body(data),
            0x0002 => new T2Body(data),
            0x0008 => new T8Body(data),
            0x0018 => new T18Body(data),
            0x0100 => new T100Body(data),
            // case 0x0103: return new T103Body(data);
            0x0104 => new T104Body(data),
            0x0106 => new T106Body(data),
            0x0107 => new T107Body(data),
            0x0108 => new T108Body(data),
            0x0109 => new T109Body(data),
            0x010a => new T10aBody(data),
            0x010c => new T10cBody(data),
            0x010d => new T10dBody(data),
            0x010e => new T10eBody(data),
            0x0114 => new T114Body(data),
            0x0116 => new T116Body(data),
            // case 0x0118: return new T118Body(data);
            0x0119 => new T119Body(data),
            // case 0x0120: return new T120Body(data);
            0x011a => new T11aBody(data),
            0x011d => new T11dBody(data),
            0x011f => new T11fBody(data),
            0x0124 => new T124Body(data),
            0x0128 => new T128Body(data),
            // case 0x0130: return new T130Body(data);
            0x0133 => new T133Body(data),
            0x0134 => new T134Body(data),
            0x0138 => new T138Body(data),
            0x0141 => new T141Body(data),
            0x0142 => new T142Body(data),
            0x0143 => new T143Body(data),
            0x0144 => new T144Body(data),
            0x0145 => new T145Body(data),
            0x0146 => new T146Body(data),
            0x0147 => new T147Body(data),
            0x0148 => new T148Body(data),
            0x0153 => new T153Body(data),
            0x0154 => new T154Body(data),
            0x0161 => new T161Body(data),
            // case 0x0163: return new T163Body(data);
            0x0166 => new T166Body(data),
            0x016a => new T16aBody(data),
            // case 0x016d: return new T16dBody(data);
            0x016e => new T16eBody(data),
            0x0174 => new T174Body(data),
            0x0177 => new T177Body(data),
            0x0178 => new T178Body(data),
            0x0179 => new T179Body(data),
            0x017a => new T17aBody(data),
            0x017b => new T17bBody(data),
            0x017c => new T17cBody(data),
            0x017d => new T17dBody(data),
            0x017e => new T17eBody(data),
            0x0187 => new T187Body(data),
            0x0188 => new T188Body(data),
            0x0191 => new T191Body(data),
            0x0192 => new T192Body(data),
            0x0193 => new T193Body(data),
            0x0194 => new T194Body(data),
            0x0197 => new T197Body(data),
            0x0198 => new T198Body(data),
            0x0202 => new T202Body(data),
            // case 0x0203: return new T203Body(data);
            0x0204 => new T204Body(data),
            0x0305 => new T305Body(data),
            0x0318 => new T318Body(data),
            // case 0x0322: return new T322Body(data);
            0x0401 => new T401Body(data),
            // case 0x0403: return new T403Body(data);
            0x0508 => new T508Body(data),
            0x0511 => new T511Body(data),
            // case 0x0512: return new T512Body(data);
            0x0516 => new T516Body(data),
            0x0521 => new T521Body(data),
            // case 0x0522: return new T522Body(data);
            // case 0x0537: return new T537Body(data);
            0x0525 => new T525Body(data),
            // case 0x0528: return new T528Body(data);
            0x052d => new T52dBody(data),
            0x0536 => new T536Body(data),
            0x0544 => new T544Body(data),
            0x0545 => new T545Body(data),
            0x0546 => new T546Body(data),
            // 0x0547 => new T547Body(data),
            _ => null
        };
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
