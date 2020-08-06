using ProtoBuf;

namespace Konata.Protocol.Protobuf
{
    [ProtoContract(SkipConstructor = true)]

    public class DeviceReport
    {
        [ProtoMember(1)]
        public byte[] Bootloader { get; set; }


        [ProtoMember(2)]
        public byte[] Version { get; set; }


        [ProtoMember(3)]
        public byte[] CodeName { get; set; }


        [ProtoMember(4)]
        public byte[] Incremental { get; set; }


        [ProtoMember(5)]
        public byte[] Fingerprint { get; set; }


        [ProtoMember(6)]
        public byte[] BootId { get; set; }


        [ProtoMember(7)]
        public byte[] AndroidId { get; set; }


        [ProtoMember(8)]
        public byte[] BaseBand { get; set; }


        [ProtoMember(9)]
        public byte[] InnerVersion { get; set; }

    }
}
