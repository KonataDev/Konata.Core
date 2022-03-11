using Konata.Core.Utils.Crypto;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Packets.Oicq
{
    public class OicqResponse : PacketBase
    {
        public uint Uin { get; }

        public ushort Version { get; }

        public ushort Command { get; }

        public OicqStatus Status { get; }

        public PacketBase BodyData { get; }

        public OicqResponse(byte[] data, byte[] shareKey) : base(data)
        {
            EatBytes(1);
            {
                EatBytes(2);
                Version = TakeUshortBE(out _);
                Command = TakeUshortBE(out _);

                EatBytes(2);
                Uin = TakeUintBE(out _);

                EatBytes(2);
                Status = (OicqStatus) TakeByte(out var _);

                // Decrypt body
                BodyData = new PacketBase(TakeBytes(out byte[] _,
                    RemainLength - 1), TeaCryptor.Instance, shareKey);
                {
                    BodyData.EatBytes(2);
                    BodyData.EatBytes(1);
                }
            }
            EatBytes(1);
        }
    }
}
