using System;
using Konata.Core.Packets.Tlv;
using Konata.Core.Utils.Ecdh;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Packets.Oicq;

internal class OicqResponse : PacketBase
{
    public uint Uin { get; }

    public ushort Version { get; }

    public ushort Command { get; }

    public OicqStatus Status { get; }

    public PacketBase BodyData { get; }

    public OicqResponse(byte[] data, EcdhCryptor cryptor) : base(data)
    {
        EatBytes(1);
        {
            EatBytes(2);
            Version = TakeUshortBE(out _);
            Command = TakeUshortBE(out _);

            EatBytes(2);
            Uin = TakeUintBE(out _);

            // Key status
            var keyStat = (OicqShareKeyStat) TakeUshortBE(out _);
            Status = (OicqStatus) TakeByte(out _);

            // Decrypt body
            BodyData = new PacketBase
                (TakeBytes(out _, RemainLength - 1), cryptor);
            {
                switch (keyStat)
                {
                    case OicqShareKeyStat.NoNeed:
                        BodyData.EatBytes(2);
                        BodyData.EatBytes(1);
                        break;

                    case OicqShareKeyStat.TwoStepExchange:
                        var bobPublic = BodyData.TakeBytes(out _, Prefix.Uint16);
                        cryptor.GenerateShared(bobPublic);

                        // Decrypt data
                        BodyData = new PacketBase(BodyData.TakeAllBytes(out _), cryptor);
                        BodyData.EatBytes(2);
                        BodyData.EatBytes(1);
                        break;
                }
            }
        }
        EatBytes(1);
    }
}
