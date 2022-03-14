using System.Collections.Generic;
using Konata.Core.Common;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace Konata.Core.Packets.SvcResponse;

internal class SvcRspGetTroopMemberListResp : UniPacket
{
    public uint SelfUin { get; private set; }

    public ulong GroupCode { get; private set; }

    public uint GroupUin { get; private set; }

    public List<BotMember> Members { get; private set; }

    public uint NextUin { get; private set; }

    public int Result { get; private set; }

    public short ErrorCode { get; private set; }

    public uint OfficeMode { get; private set; }

    public uint NextGetTime { get; private set; }

    public SvcRspGetTroopMemberListResp(byte[] payload)
        : base(payload, (userdata, r) =>
        {
            var p = (SvcRspGetTroopMemberListResp) userdata;
            p.Members = new();

            // Read fields
            p.SelfUin = (uint) (JNumber) r["0.0"];
            p.GroupCode = (ulong) (long) (JNumber) r["0.1"];
            p.GroupUin = (uint) (JNumber) r["0.2"];
            p.NextUin = (uint) (JNumber) r["0.4"];
            p.Result = (int) (JNumber) r["0.5"];
            p.ErrorCode = (short) (JNumber) r["0.6"];
            p.OfficeMode = (uint) (JNumber) r["0.7"];
            p.NextGetTime = (uint) (JNumber) r["0.8"];

            // Read member list
            foreach (JStruct i in (JList) r["0.3"])
            {
                // Parse one member
                var member = new BotMember
                {
                    Uin = (uint) (JNumber) i["0"],
                    FaceId = (byte) (JNumber) i["1"],
                    Age = (uint) (JNumber) i["2"],
                    Gender = (byte) (JNumber) i["3"],
                    NickName = (string) (JString) i["4"],
                    Level = (int) (JNumber) i["14"],
                    JoinTime = (uint) (JNumber) i["15"],
                    LastSpeakTime = (uint) (JNumber) i["16"],
                    IsAdmin = ((uint) (JNumber) i["18"]) % 2 == 1,
                    SpecialTitle = (string) (JString) i["23"],
                    SpecialTitleExpiredTime = (uint) (JNumber) i["24"],
                    MuteTimestamp = (uint) (JNumber) i["30"],
                };

                // Add member list
                p.Members.Add(member);
            }
        })
    {
    }
}
