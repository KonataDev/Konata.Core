using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlv
{
    public abstract class TlvBase : Packet
    {
        public readonly ushort _tlvCommand;
        public readonly ushort _tlvBodyLength;

        public TlvBody _tlvBody { get; protected set; }

        public TlvBase(ushort tlvCommand, TlvBody tlvBody)
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

        public TlvBase(ushort tlvCommand, TlvBody tlvBody, byte[] cryptKey)
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

        public TlvBase(byte[] data) : base()
        {
            TakeUshortBE(out _tlvCommand);
            TakeUshortBE(out _tlvBodyLength);
        }

        public TlvBase(byte[] data, byte[] cryptKey)
            : base(data, TeaCryptor.Instance, cryptKey)
        {
            TakeUshortBE(out _tlvCommand);
            TakeUshortBE(out _tlvBodyLength);
        }
    }

    public class TlvBody : Packet
    {
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
