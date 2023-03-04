using Konata.Core.Common;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Crypto;

// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Packets.Oicq;

internal enum OicqStatus
{
    OK = 0,

    DoVerifySmsCaptcha = 160,
    DoVerifySliderCaptcha = 2,

    DoVerifyDeviceLock = 204,
    DoVerifyDeviceLockViaSmsNewArea = 239,

    PreventByIncorrectPassword = 1,
    PreventByReceiveIssue = 3,
    PreventByTokenExpired = 15,
    PreventByAccountBanned = 40,
    PreventByOperationTimeout = 155,
    PreventBySmsSentFailed = 162,
    PreventByIncorrectSmsCode = 163,
    PreventByLoginDenied = 167,
    PreventByOutdatedVersion = 235,
    PreventByHighRiskOfEnvironment = 237,
}

internal enum OicqShareKeyStat
{
    NoNeed = 0,
    TwoStepExchange = 4
}

internal class OicqRequest : PacketBase
{
    public delegate void OicqBodyWriter(PacketBase writer);

    public OicqRequest(ushort command, uint uin, EcdhCryptor.CryptId method,
        byte[] randKey, EcdhCryptor cryptor, AppInfo appInfo, OicqBodyWriter writer) : base()
    {
        var body = new PacketBase();

        PutByte(0x02); // Head of 0x02
        {
            EnterBarrier(Prefix.Uint16, Endian.Big, 4);
            {
                PutUshortBE(8001); // oicqVersion
                PutUshortBE(command); // oicqCommand
                PutUshortBE(1);
                PutUintBE(uin);
                PutByte(0x03);
                PutByte((byte) method);
                PutByte(0x00); // 0x00
                PutUintBE(2);
                PutUintBE(appInfo.AppClientVersion);
                PutUintBE(0);

                writer.Invoke(body);
                PutPacketEncrypted(body, cryptor, randKey);  
            }
            LeaveBarrier();
        }
        PutByte(0x03); // End with 0x03
    }
}
