using System.Collections.Generic;
using Konata.Core.Common;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable PossibleInvalidCastExceptionInForeachLoop

namespace Konata.Core.Packets.SvcResponse;

internal class SvcRspGetFriendListResp : UniPacket
{
    public uint TotalFriendCount { get; private set; }

    public List<BotFriend> Friends { get; private set; }

    public int Result { get; private set; }

    public short ErrorCode { get; private set; }

    public SvcRspGetFriendListResp(byte[] payload)
        : base(payload, (userdata, r) =>
        {
            // Initialize
            var p = (SvcRspGetFriendListResp) userdata;
            p.Friends = new();

            p.TotalFriendCount = (uint) (JNumber) r["0.5"];
            p.Result = (int) (JNumber) r["0.15"];
            p.ErrorCode = (short) (JNumber) r["0.16"];

            // Read friend list
            foreach (JStruct i in (JList) r["0.7"])
            {
                // Parse one friend
                var friend = new BotFriend
                {
                    Uin = (uint) (JNumber) i["0"],
                    FaceId = (byte) (JNumber) i["2"],
                    Name = (string) (JString) i["14"],
                    Gender = (byte) (JNumber) i["31"],
                };

                // Add to friend list
                p.Friends.Add(friend);
            }
        })
    {
    }
}
