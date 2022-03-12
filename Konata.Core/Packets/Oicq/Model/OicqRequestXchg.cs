using System;
using Konata.Core.Common;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.Model;

namespace Konata.Core.Packets.Oicq.Model
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestXchg : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x000B;

        public OicqRequestXchg(int sequence, BotKeyStore signinfo, BotDevice device)
            : base(OicqCommand, signinfo.Account.Uin, OicqEncryptMethod.ECDH7,
                signinfo.KeyStub.ShareKey, signinfo.KeyStub.RandKey,
                signinfo.KeyStub.PublicKey, w =>
                {
                    // Device info report
                    var report = new DeviceReport(
                        device.System.BootLoader,
                        device.System.Version,
                        device.Model.CodeName,
                        device.System.Incremental,
                        device.System.FingerPrint,
                        device.System.BootId,
                        device.System.AndroidId,
                        device.Model.BaseBand,
                        device.System.InnerVersion);

                    TlvPacker tlvs = new TlvPacker();
                    {
                        tlvs.PutTlv(new Tlv(0x0100, new T100Body(AppInfo.AppId,
                            0x00000064, AppInfo.AppClientVersion, 1315040)));
                        tlvs.PutTlv(new Tlv(0x10a, new T10aBody(signinfo.Session.TgtToken)));
                        tlvs.PutTlv(new Tlv(0x0116, new T116Body(AppInfo.WtLoginSdk.MiscBitmap,
                            AppInfo.WtLoginSdk.SubSigBitmap, Array.Empty<uint>())));
                        // tlvs.PutTlv(new Tlv(0x0108, new T108Body()));
                        tlvs.PutTlv(new Tlv(0x0144, new T144Body(device.System.AndroidId,
                            report,
                            device.System.Type,
                            device.System.Version,
                            device.Network.NetType,
                            device.Network.NetOperator,
                            device.Network.NetApn,
                            false, true, false,
                            device.System.Guid,
                            285212672,
                            device.Model.Name,
                            device.Model.Manufacturer), signinfo.Session.TgtKey));
                        tlvs.PutTlv(new Tlv(0x0143, new T143Body(signinfo.Session.D2Token)));
                        tlvs.PutTlv(new Tlv(0x0142, new T142Body(AppInfo.ApkPackageName)));
                        tlvs.PutTlv(new Tlv(0x0154, new T154Body(sequence)));
                        tlvs.PutTlv(new Tlv(0x0018, new T18Body(AppInfo.AppId,
                            AppInfo.AppClientVersion, signinfo.Account.Uin)));
                        tlvs.PutTlv(new Tlv(0x0141, new T141Body(device.Network.NetOperator,
                            device.Network.NetType, device.Network.NetApn)));
                        tlvs.PutTlv(new Tlv(0x0008, new T8Body()));
                        tlvs.PutTlv(new Tlv(0x0147, new T147Body(AppInfo.AppId,
                            AppInfo.ApkVersionName, AppInfo.ApkSignatureMd5)));
                        tlvs.PutTlv(new Tlv(0x0177, new T177Body(AppInfo.WtLoginSdk.SdkBuildTime, AppInfo.WtLoginSdk.SdkVersion)));
                        tlvs.PutTlv(new Tlv(0x0187, new T187Body(device.Network.WifiMacAddress, 0)));
                        tlvs.PutTlv(new Tlv(0x0188, new T188Body(device.System.AndroidId)));
                        tlvs.PutTlv(new Tlv(0x0202, new T202Body(device.Network.WifiBssid,
                            device.Network.WifiSsid)));
                    }

                    w.PutUshortBE(OicqSubCommand);
                    w.PutBytes(tlvs.GetBytes(true));
                })
        {
        }
    }
}
