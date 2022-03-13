using System;
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
        Console.WriteLine("--------------------------");
        Console.WriteLine(cryptor.ShareKey.ToHex());
        Console.WriteLine("--------------------------");

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

                    case OicqShareKeyStat.ExchangeTwice:
                        // BodyData.TakeBytes(out var bobPublic, Prefix.Uint16);
                        // var shared = cryptor.GenerateShared(bobPublic);
                        throw new NotSupportedException("Not supported to calculate share key twice.");
                        break;
                }
            }
        }
        EatBytes(1);
    }
}
