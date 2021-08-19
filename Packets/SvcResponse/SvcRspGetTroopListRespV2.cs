using System;
using System.Collections.Generic;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable PossibleInvalidCastExceptionInForeachLoop

namespace Konata.Core.Packets.SvcResponse
{
    public class SvcRspGetTroopListRespV2 : UniPacket
    {
        public List<BotGroup> Groups { get; private set; }

        public SvcRspGetTroopListRespV2(byte[] payload)
            : base(payload, (userdata, r) =>
            {
                // Initialize
                var p = (SvcRspGetTroopListRespV2) userdata;
                p.Groups = new();

                // Read group list
                foreach (JStruct i in (JList) r["0.5"])
                {
                    // Parse one group
                    var group = new BotGroup
                    {
                        Uin = (uint) (JNumber) i["0"],
                        Code = (ulong) (long) (JNumber) i["1"],
                        Name = (string) (JString) i["4"],
                        MemberCount = (uint) (JNumber) i["19"],
                        MaxMemberCount = (uint) (JNumber) i["29"],
                        OwnerUin = (uint) (JNumber) i["23"],
                        Muted = (uint) (JNumber) i["9"],
                        MutedMe = (uint) (JNumber) i["10"] * 1000,
                    };

                    // Some conditions
                    if (group.Muted == 1)
                    {
                        group.Muted = UInt32.MaxValue;
                    }

                    if (group.MutedMe < DateTime.UtcNow.Ticks)
                    {
                        group.MutedMe = 0;
                    }

                    // Add to group list
                    p.Groups.Add(group);
                }
            })
        {
        }
    }
}
