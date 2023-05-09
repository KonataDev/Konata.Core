using System;
using Konata.Core.Common;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Tencent.QImei;

namespace Konata.Core.Packets.Tlv.Model;

internal class T545Body : TlvBody
{
    public T545Body(BotDevice device, AppInfo appInfo)
        : base()
    {
        var (qImei16, _) = QImeiProvider.RequestQImei(device, appInfo).Result;
        PutBytes(qImei16 != null ? ByteConverter.UnHex(qImei16) : ByteConverter.UnHex(device.Model.Imei));
    }

    public T545Body(byte[] data)
        : base(data)
    {
        EatBytes(RemainLength);
    }
}
