using System;
using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[EventSubscribe(typeof(PbGetMessageEvent))]
[Service("MessageSvc.PbGetMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbGetMsg : BaseService<PbGetMessageEvent>
{
	protected override bool Parse(SSOFrame input,
		 BotKeyStore keystore, out PbGetMessageEvent output)
	{
		var root = ProtoTreeRoot.Deserialize(input.Payload, true);

		// Get sync cookie 
		root.TryGetLeafBytes("1A", out var cookie);

		// Get push events
		var push = new List<ProtocolEvent>();

		var root2A = root.GetLeaves<ProtoTreeRoot>("2A");
		if (root2A == null) goto Finish;

		foreach (var i in root2A)
		{
			var root22 = i.GetLeaves<ProtoTreeRoot>("22");
			if (root22 == null) continue;

			foreach (var j in root22)
			{
				j.GetTree("0A", _ =>
				{
					var type = (NotifyType)_.GetLeafVar("18");
					switch (type)
					{
						case NotifyType.FriendMessage:
						case NotifyType.FriendMessageSingle:
						case NotifyType.FriendPttMessage:
						case NotifyType.StrangerMessage:
							push.Add(OnProcessMessage(keystore.Account.Uin, j));
							break;

						default:
						case NotifyType.FriendFileMessage:
						case NotifyType.NewMember:
						case NotifyType.GroupCreated:
						case NotifyType.GroupRequestAccepted:
							break;
					}
				});
			}
		}

	Finish:
		output = PbGetMessageEvent.Result(0, cookie, push);
		return true;
	}

	protected override bool Build(int sequence, PbGetMessageEvent input,
		 BotKeyStore keystore, BotDevice device, ref PacketBase output)
	{
		output.PutProtoNode(new GetMessageRequest(input.SyncCookie));
		return true;
	}

	private FriendMessageEvent OnProcessMessage(uint selfUin, ProtoTreeRoot root)
	{
		var sourceRoot = root.PathTo<ProtoTreeRoot>("0A");

		var toUin = (uint)sourceRoot.GetLeafVar("10");
		var fromUin = (uint)sourceRoot.GetLeafVar("08");
		var friendUin = (toUin == selfUin) ? fromUin : toUin;
		
		var message = MessagePacker.UnPack(root
				.PathTo<ProtoTreeRoot>("1A.0A"), MessagePacker.ParseMode.Friend);

		return FriendMessageEvent.Push(fromUin, selfUin, message);
	}
}
