using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.OfflineFilleHandleSvr;

/// <summary>
/// OfflineFilleHandleSvr.pb_ftn_CMD_REQ_APPLY_DOWNLOAD-1200 (这个包确实是这个名字 可能是__的typo)
/// </summary>
[EventSubscribe(typeof(OfflineFileDownloadEvent))]
[Service("OfflineFilleHandleSvr.pb_ftn_CMD_REQ_APPLY_DOWNLOAD-1200", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbFtnCmdReqApplyDownload1200 : BaseService<OfflineFileDownloadEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo, BotKeyStore keystore, 
        out OfflineFileDownloadEvent output)
    {
        var fileRoot = ProtoTreeRoot.Deserialize(input.Payload, true)
            .GetLeaf<ProtoTreeRoot>("72").GetLeaf<ProtoTreeRoot>("F201");
        var domain = fileRoot.GetLeafString("D205");
        var path = fileRoot.GetLeafString("9203");
        output = OfflineFileDownloadEvent.Result($"https://{domain}{path}");
        return true;
    }

    protected override bool Build(int sequence, OfflineFileDownloadEvent input, AppInfo appInfo, BotKeyStore keystore, 
        BotDevice device, ref PacketBase output)
    {
        output.PutProtoNode(new SvcReqGetOfflineDownloadUrl(sequence, input.SelfUin, input.FileUuid));
        return true;
    }
}