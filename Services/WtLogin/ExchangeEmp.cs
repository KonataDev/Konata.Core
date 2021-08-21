using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Packets.Oicq;
using Konata.Core.Packets.Oicq.Model;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.Model;
using Konata.Core.Utils.Crypto;

// ReSharper disable UnusedVariable
// ReSharper disable InvertIf
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Konata.Core.Services.WtLogin
{
    [EventSubscribe(typeof(WtLoginEvent))]
    [Service("wtlogin.exchange_emp", "Exchange token")]
    public class ExchangeEmp : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            // Parse oicq response
            var oicqResponse = new OicqResponse
                (input.Payload.GetBytes(), keystore.KeyStub.ShareKey);

            // Select status
            output = oicqResponse.Status switch
            {
                OicqStatus.OK => OnRecvXchgSuccess(oicqResponse, keystore),
                OicqStatus.PreventByTokenExpired => OnRecvTokenExpired(oicqResponse, keystore),
                _ => OnRecvUnknown(oicqResponse)
            };

            return true;
        }

        /// <summary>
        /// Xchg success
        /// </summary>
        /// <param name="response"></param>
        /// <param name="keystore"></param>
        /// <returns></returns>
        private WtLoginEvent OnRecvXchgSuccess(OicqResponse response, BotKeyStore keystore)
        {
            var tlvs = response.BodyData.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            if (unpacker.Count == 1)
            {
                Tlv tlv119 = unpacker.TryGetTlv(0x119);
                if (tlv119 != null)
                {
                    var decrypted = tlv119._tlvBody.TakeDecryptedBytes(out var _,
                        TeaCryptor.Instance, keystore.KeyStub.TgtgKey);

                    var tlv119Unpacker = new TlvUnpacker(decrypted, true);

                    Tlv tlv16a = tlv119Unpacker.TryGetTlv(0x16a); // no pic sig
                    Tlv tlv10c = tlv119Unpacker.TryGetTlv(0x10c); // gt key
                    Tlv tlv10a = tlv119Unpacker.TryGetTlv(0x10a); // tgt
                    Tlv tlv10d = tlv119Unpacker.TryGetTlv(0x10d); // tgt key
                    Tlv tlv114 = tlv119Unpacker.TryGetTlv(0x114); // st
                    Tlv tlv10e = tlv119Unpacker.TryGetTlv(0x10e); // st key
                    Tlv tlv103 = tlv119Unpacker.TryGetTlv(0x103); // stwx_web
                    Tlv tlv133 = tlv119Unpacker.TryGetTlv(0x133);
                    Tlv tlv134 = tlv119Unpacker.TryGetTlv(0x134); // ticket key
                    Tlv tlv322 = tlv119Unpacker.TryGetTlv(0x322); // device token
                    Tlv tlv11d = tlv119Unpacker.TryGetTlv(0x11d); // st, st key
                    Tlv tlv11a = tlv119Unpacker.TryGetTlv(0x11a); // age, sex, nickname
                    Tlv tlv120 = tlv119Unpacker.TryGetTlv(0x120); // skey
                    Tlv tlv512 = tlv119Unpacker.TryGetTlv(0x512); // Map<domain, p_skey>
                    Tlv tlv305 = tlv119Unpacker.TryGetTlv(0x305); // d2key
                    Tlv tlv143 = tlv119Unpacker.TryGetTlv(0x143); // d2

                    var noPicSig = ((T16aBody) tlv16a._tlvBody)._noPicSig;

                    var tgtKey = ((T10dBody) tlv10d._tlvBody)._tgtKey;
                    var tgtToken = ((T10aBody) tlv10a._tlvBody)._tgtToken;

                    var d2Key = ((T305Body) tlv305._tlvBody)._d2Key;
                    var d2Token = ((T143Body) tlv143._tlvBody)._d2Token;

                    var wtSessionTicketSig = ((T133Body) tlv133._tlvBody)._wtSessionTicketSig;
                    var wtSessionTicketKey = ((T134Body) tlv134._tlvBody)._wtSessionTicketKey;

                    var gtKey = ((T10cBody) tlv10c._tlvBody)._gtKey;
                    var stKey = ((T10eBody) tlv10e._tlvBody)._stKey;

                    var userAge = ((T11aBody) tlv11a._tlvBody)._age;
                    var userFace = ((T11aBody) tlv11a._tlvBody)._face;
                    var userNickname = ((T11aBody) tlv11a._tlvBody)._nickName;

                    // TODO: cleanup keys
                    keystore.Session.TgtKey = tgtKey;
                    keystore.Session.TgtToken = tgtToken;
                    keystore.Session.D2Key = d2Key;
                    keystore.Session.D2Token = d2Token;
                    keystore.Session.WtSessionTicketSig = wtSessionTicketSig;
                    keystore.Session.WtSessionTicketKey = wtSessionTicketKey;
                    keystore.Session.GtKey = gtKey;
                    keystore.Session.StKey = stKey;

                    return WtLoginEvent.ResultOk((int) response.Status);
                }
            }

            return OnRecvUnknown(response);
        }

        /// <summary>
        /// Token expired
        /// </summary>
        /// <param name="response"></param>
        /// <param name="keystore"></param>
        /// <returns></returns>
        private WtLoginEvent OnRecvTokenExpired(OicqResponse response, BotKeyStore keystore)
            => WtLoginEvent.ResultTokenExpired((int) response.Status);


        /// <summary>
        /// Unknown code
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private WtLoginEvent OnRecvUnknown(OicqResponse response)
            => WtLoginEvent.ResultUnknown((int) response.Status, "Unknown OicqRequest received.");

        public bool Build(Sequence sequence, WtLoginEvent input, BotKeyStore keystore,
            BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.GetSessionSequence("wtlogin.exchange_emp");

            // TODO:
            // Move this to logic layer

            // Update keys
            keystore.Session.TgtKey =
                new Md5Cryptor().Encrypt(keystore.Session.D2Key);

            OicqRequest oicqRequest;

            // Build OicqRequest
            switch (input.EventType)
            {
                case WtLoginEvent.Type.Xchg:
                    oicqRequest = new OicqRequestXchg(newSequence, keystore, device);
                    break;

                default:
                    return false;
            }

            // Build to service
            if (SSOFrame.Create("wtlogin.exchange_emp", PacketType.TypeA,
                newSequence, keystore.Session.TgtToken,
                sequence.Session, oicqRequest, out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.WtLoginExchange,
                    keystore.Account.Uin, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int outsequence, out byte[] output)
            => Build(sequence, (WtLoginEvent) input, keystore, device, out outsequence, out output);
    }
}
