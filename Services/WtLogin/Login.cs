using System;

using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.TlvModel;
using Konata.Core.Packets.Oicq;
using Konata.Core.Attributes;

using Konata.Utils.IO;
using Konata.Utils.Crypto;

namespace Konata.Core.Services.WtLogin
{
    [Service("wtlogin.login", "WtLogin exchange")]
    [EventDepends(typeof(WtLoginEvent))]
    public class Login : IService
    {
        public bool Parse(SSOFrame ssoFrame, BotKeyStore signinfo, out ProtocolEvent output)
        {
            var oicqRequest = new OicqRequest(ssoFrame.Payload.GetBytes(), signinfo.KeyStub.ShareKey);

            Console.WriteLine($"  [oicqRequest] oicqCommand => {oicqRequest.oicqCommand}");
            Console.WriteLine($"  [oicqRequest] oicqVersion => {oicqRequest.oicqVersion}");
            Console.WriteLine($"  [oicqRequest] oicqStatus => {oicqRequest.oicqStatus}");

            switch (oicqRequest.oicqStatus)
            {
                case OicqStatus.OK:
                    output = OnRecvWtloginSuccess(oicqRequest, signinfo); break;

                case OicqStatus.DoVerifySliderCaptcha:
                    output = OnRecvCheckSliderCaptcha(oicqRequest, signinfo); break;
                case OicqStatus.DoVerifySms:
                    output = OnRecvCheckSmsCaptcha(oicqRequest, signinfo); break;

                case OicqStatus.PreventByIncorrectUserOrPwd:
                    output = OnRecvInvalidUsrPwd(oicqRequest, signinfo); break;
                case OicqStatus.PreventByIncorrectSmsCode:
                    output = OnRecvInvalidSmsCode(oicqRequest, signinfo); break;
                case OicqStatus.PreventByInvalidEnvironment:
                    output = OnRecvInvalidLoginEnv(oicqRequest, signinfo); break;
                case OicqStatus.PreventByLoginDenied:
                    output = OnRecvLoginDenied(oicqRequest, signinfo); break;

                default:
                    output = OnRecvUnknown(); break;
            }

            return true;
        }

        #region Event Handlers

        private ProtocolEvent OnRecvCheckSliderCaptcha(OicqRequest request, BotKeyStore signinfo)
        {
            Console.WriteLine("  Do slider verification.");

            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv104 = unpacker.TryGetTlv(0x104);
            Tlv tlv192 = unpacker.TryGetTlv(0x192);
            if (tlv104 != null && tlv192 != null)
            {
                var sigSession = ((T104Body)tlv104._tlvBody)._sigSession;
                var sigCaptchaURL = ((T192Body)tlv192._tlvBody)._url;

                signinfo.Session.WtLoginSession = sigSession;

                return new WtLoginEvent
                {
                    SliderURL = sigCaptchaURL,
                    EventType = WtLoginEvent.Type.CheckSlider
                };
            }

            return OnRecvUnknown();
        }

        private ProtocolEvent OnRecvCheckSmsCaptcha(OicqRequest request, BotKeyStore signinfo)
        {
            Console.WriteLine("  Do sms verification.");

            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            if (unpacker.Count == 8 || unpacker.Count == 9)
            {
                Tlv tlv104 = unpacker.TryGetTlv(0x104);
                Tlv tlv174 = unpacker.TryGetTlv(0x174);
                Tlv tlv204 = unpacker.TryGetTlv(0x204);
                Tlv tlv178 = unpacker.TryGetTlv(0x178);
                Tlv tlv179 = unpacker.TryGetTlv(0x179);
                Tlv tlv17d = unpacker.TryGetTlv(0x17d);
                Tlv tlv402 = unpacker.TryGetTlv(0x402);
                Tlv tlv403 = unpacker.TryGetTlv(0x403);
                Tlv tlv17e = unpacker.TryGetTlv(0x17e);

                if (tlv104 != null && tlv174 != null
                    && tlv204 != null && tlv178 != null
                    && tlv17d != null && tlv402 != null
                    && tlv403 != null && tlv17e != null)
                {
                    var sigSession = ((T104Body)tlv104._tlvBody)._sigSession;
                    var sigMessage = ((T17eBody)tlv17e._tlvBody)._message;
                    var smsPhone = ((T178Body)tlv178._tlvBody)._phone;
                    var smsCountryCode = ((T178Body)tlv178._tlvBody)._countryCode;
                    var smsToken = ((T174Body)tlv174._tlvBody)._smsToken;

                    signinfo.Session.WtLoginSession = sigSession;
                    signinfo.Session.WtLoginSmsPhone = smsPhone;
                    signinfo.Session.WtLoginSmsToken = smsToken;
                    signinfo.Session.WtLoginSmsCountry = smsCountryCode;

                    return new WtLoginEvent
                    {
                        EventType = WtLoginEvent.Type.RefreshSMS
                    };
                }
            }
            else if (unpacker.Count == 2)
            {
                Tlv tlv104 = unpacker.TryGetTlv(0x104);
                Tlv tlv17b = unpacker.TryGetTlv(0x17b);

                if (tlv104 != null && tlv17b != null)
                {
                    var sigSession = ((T104Body)tlv104._tlvBody)._sigSession;

                    signinfo.Session.WtLoginSession = sigSession;

                    return new WtLoginEvent
                    {
                        SmsPhone = signinfo.Session.WtLoginSmsPhone,
                        SmsCountry = signinfo.Session.WtLoginSmsCountry,
                        EventType = WtLoginEvent.Type.CheckSMS
                    };
                }
            }

            return OnRecvUnknown();
        }

        private ProtocolEvent OnRecvResponseVerifyImageCaptcha(OicqRequest request, BotKeyStore signinfo)
        {
            // <TODO> Image captcha

            return new WtLoginEvent
            {
                EventType = WtLoginEvent.Type.NotImplemented,
                EventMessage = "Image captcha not implemented."
            };
        }

        private ProtocolEvent OnRecvResponseVerifyDeviceLock(OicqRequest request, BotKeyStore signinfo)
        {
            // <TODO> Device lock

            return new WtLoginEvent
            {
                EventType = WtLoginEvent.Type.CheckDevLock,
                EventMessage = "DeviceLock not implemented. Please turn off your device lock and try again."
            };
        }

        private ProtocolEvent OnRecvWtloginSuccess(OicqRequest request, BotKeyStore signinfo)
        {
            Console.WriteLine("  Wtlogin success.");

            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            if (unpacker.Count == 2)
            {
                Tlv tlv119 = unpacker.TryGetTlv(0x119);
                Tlv tlv161 = unpacker.TryGetTlv(0x161);

                if (tlv119 != null && tlv161 != null)
                {
                    var decrypted = tlv119._tlvBody.TakeDecryptedBytes(out var _,
                        TeaCryptor.Instance, signinfo.KeyStub.TgtgKey);

                    var tlv119Unpacker = new TlvUnpacker(decrypted, true);

                    Tlv tlv16a = tlv119Unpacker.TryGetTlv(0x16a); // no pic sig
                    Tlv tlv106 = tlv119Unpacker.TryGetTlv(0x106);
                    Tlv tlv10c = tlv119Unpacker.TryGetTlv(0x10c); // gt key
                    Tlv tlv10a = tlv119Unpacker.TryGetTlv(0x10a); // tgt
                    Tlv tlv10d = tlv119Unpacker.TryGetTlv(0x10d); // tgt key
                    Tlv tlv114 = tlv119Unpacker.TryGetTlv(0x114); // st
                    Tlv tlv10e = tlv119Unpacker.TryGetTlv(0x10e); // st key
                    Tlv tlv103 = tlv119Unpacker.TryGetTlv(0x103); // stwx_web
                    Tlv tlv133 = tlv119Unpacker.TryGetTlv(0x133);
                    Tlv tlv134 = tlv119Unpacker.TryGetTlv(0x134); // ticket key
                    Tlv tlv528 = tlv119Unpacker.TryGetTlv(0x528);
                    Tlv tlv322 = tlv119Unpacker.TryGetTlv(0x322); // device token
                    Tlv tlv11d = tlv119Unpacker.TryGetTlv(0x11d); // st, st key
                    Tlv tlv11f = tlv119Unpacker.TryGetTlv(0x11f);
                    Tlv tlv138 = tlv119Unpacker.TryGetTlv(0x138);
                    Tlv tlv11a = tlv119Unpacker.TryGetTlv(0x11a); // age, sex, nickname
                    Tlv tlv522 = tlv119Unpacker.TryGetTlv(0x522);
                    Tlv tlv537 = tlv119Unpacker.TryGetTlv(0x537);
                    Tlv tlv550 = tlv119Unpacker.TryGetTlv(0x550);
                    Tlv tlv203 = tlv119Unpacker.TryGetTlv(0x203);
                    Tlv tlv120 = tlv119Unpacker.TryGetTlv(0x120); // skey
                    Tlv tlv16d = tlv119Unpacker.TryGetTlv(0x16d);
                    Tlv tlv512 = tlv119Unpacker.TryGetTlv(0x512); // Map<domain, p_skey>
                    Tlv tlv305 = tlv119Unpacker.TryGetTlv(0x305); // d2key
                    Tlv tlv143 = tlv119Unpacker.TryGetTlv(0x143); // d2
                    Tlv tlv118 = tlv119Unpacker.TryGetTlv(0x118);
                    Tlv tlv163 = tlv119Unpacker.TryGetTlv(0x163);
                    Tlv tlv130 = tlv119Unpacker.TryGetTlv(0x130);
                    Tlv tlv403 = tlv119Unpacker.TryGetTlv(0x403);

                    var noPicSig = ((T16aBody)tlv16a._tlvBody)._noPicSig;

                    var tgtKey = ((T10dBody)tlv10d._tlvBody)._tgtKey;
                    var tgtToken = ((T10aBody)tlv10a._tlvBody)._tgtToken;

                    var d2Key = ((T305Body)tlv305._tlvBody)._d2Key;
                    var d2Token = ((T143Body)tlv143._tlvBody)._d2Token;

                    var wtSessionTicketSig = ((T133Body)tlv133._tlvBody)._wtSessionTicketSig;
                    var wtSessionTicketKey = ((T134Body)tlv134._tlvBody)._wtSessionTicketKey;

                    var gtKey = ((T10cBody)tlv10c._tlvBody)._gtKey;
                    var stKey = ((T10eBody)tlv10e._tlvBody)._stKey;

                    var userAge = ((T11aBody)tlv11a._tlvBody)._age;
                    var userFace = ((T11aBody)tlv11a._tlvBody)._face;
                    var userNickname = ((T11aBody)tlv11a._tlvBody)._nickName;

                    signinfo.Session.TgtKey = tgtKey;
                    signinfo.Session.TgtToken = tgtToken;
                    signinfo.Session.D2Key = d2Key;
                    signinfo.Session.D2Token = d2Token;
                    signinfo.Session.WtSessionTicketSig = wtSessionTicketSig;
                    signinfo.Session.WtSessionTicketKey = wtSessionTicketKey;
                    signinfo.Session.GtKey = gtKey;
                    signinfo.Session.StKey = stKey;
                    signinfo.Account.Age = userAge;
                    signinfo.Account.Face = userFace;
                    signinfo.Account.Name = userNickname;
                    signinfo.Account.Age = userAge;

                    Console.WriteLine($"  [SignInfo] gtKey => { ByteConverter.Hex(signinfo.Session.GtKey, true) }");
                    Console.WriteLine($"  [SignInfo] stKey => { ByteConverter.Hex(signinfo.Session.StKey, true) }");
                    Console.WriteLine($"  [SignInfo] tgtKey => { ByteConverter.Hex(signinfo.Session.TgtKey, true) }");
                    Console.WriteLine($"  [SignInfo] tgtToken => { ByteConverter.Hex(signinfo.Session.TgtToken, true) }");
                    Console.WriteLine($"  [SignInfo] d2Key => { ByteConverter.Hex(signinfo.Session.D2Key, true) }");
                    Console.WriteLine($"  [SignInfo] d2Token => { ByteConverter.Hex(signinfo.Session.D2Token, true) }");

                    Console.WriteLine($"  [UinInfo] Uin => { signinfo.Account.Uin }");
                    Console.WriteLine($"  [UinInfo] Name => { signinfo.Account.Name }");

                    return new WtLoginEvent
                    {
                        EventType = WtLoginEvent.Type.OK
                    };
                }
            }

            return OnRecvUnknown();
        }

        private ProtocolEvent OnRecvInvalidUsrPwd(OicqRequest request, BotKeyStore signinfo)
        {
            return new WtLoginEvent
            {
                EventType = WtLoginEvent.Type.InvalidUinOrPassword,
                EventMessage = "Incorrect account or password."
            };
        }

        private ProtocolEvent OnRecvInvalidSmsCode(OicqRequest request, BotKeyStore signinfo)
        {
            return new WtLoginEvent
            {
                EventType = WtLoginEvent.Type.InvalidSmsCode,
                EventMessage = "Incorrect sms code."
            };
        }

        private ProtocolEvent OnRecvInvalidLoginEnv(OicqRequest request, BotKeyStore signinfo)
        {
            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv146 = unpacker.TryGetTlv(0x146);
            if (tlv146 != null)
            {
                var errorTitle = ((T146Body)tlv146._tlvBody)._title;
                var errorMessage = ((T146Body)tlv146._tlvBody)._message;

                return new WtLoginEvent
                {
                    EventType = WtLoginEvent.Type.InvalidLoginEnvironment,
                    EventMessage = $"{errorTitle} {errorMessage}"
                };
            }

            return OnRecvUnknown();
        }

        private ProtocolEvent OnRecvLoginDenied(OicqRequest request, BotKeyStore signinfo)
        {
            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv146 = unpacker.TryGetTlv(0x146);
            if (tlv146 != null)
            {
                var errorTitle = ((T146Body)tlv146._tlvBody)._title;
                var errorMessage = ((T146Body)tlv146._tlvBody)._message;

                return new WtLoginEvent
                {
                    EventType = WtLoginEvent.Type.LoginDenied,
                    EventMessage = $"{errorTitle} {errorMessage}"
                };
            }

            return OnRecvUnknown();
        }

        private ProtocolEvent OnRecvUnknown()
        {
            return new WtLoginEvent
            {
                EventType = WtLoginEvent.Type.Unknown,
                EventMessage = "Unknown OicqRequest received."
            };
        }

        #endregion

        public bool Build(Sequence sequence, WtLoginEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequece, out byte[] output)
        {
            output = null;
            newSequece = sequence.GetSessionSequence("wtlogin.login");

            OicqRequest oicqRequest;

            // Build OicqRequest
            switch (input.EventType)
            {
                case WtLoginEvent.Type.Tgtgt:
                    oicqRequest = BuildRequestTgtgt(newSequece, signInfo, device);
                    break;

                case WtLoginEvent.Type.CheckSMS:
                    oicqRequest = BuildRequestCheckSms(input.CaptchaResult, signInfo);
                    break;

                case WtLoginEvent.Type.RefreshSMS:
                    oicqRequest = BuildRequestRefreshSms(signInfo);
                    break;

                case WtLoginEvent.Type.CheckSlider:
                    oicqRequest = BuildRequestCheckSlider(input.CaptchaResult, signInfo);
                    break;

                default:
                    return false;
            }

            // Build to service
            if (SSOFrame.Create("wtlogin.login", PacketType.TypeA,
                newSequece, sequence.Session, oicqRequest, out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.WtLoginExchange,
                    signInfo.Account.Uin, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signinfo, BotDevice device, out int outsequence, out byte[] output)
            => Build(sequence, (WtLoginEvent)input, signinfo, device, out outsequence, out output);

        #region Event Builders

        private OicqRequest BuildRequestTgtgt(int sequence, BotKeyStore signinfo,
            BotDevice device)
            => new OicqRequestTgtgt(sequence, signinfo, device);

        private OicqRequest BuildRequestCheckSms(string code, BotKeyStore signinfo)
            => new OicqRequestCheckSms(code, signinfo);

        private OicqRequest BuildRequestCheckSlider(string ticket, BotKeyStore signinfo)
            => new OicqRequestCheckImage(ticket, signinfo);

        private OicqRequest BuildRequestRefreshSms(BotKeyStore signinfo)
            => new OicqRequestRefreshSms(signinfo);

        #endregion
    }
}
