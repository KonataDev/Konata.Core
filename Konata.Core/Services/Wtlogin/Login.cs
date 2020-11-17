using System;
using Konata.Utils;
using Konata.Events;
using Konata.Crypto;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.Tlv;
using Konata.Packets.Oicq;

namespace Konata.Services.Wtlogin
{
    public class Login : ServiceRoutine
    {
        public Login(EventPumper eventPumper)
            : base("wtlogin.login", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            if (eventParacel is EventWtLoginExchange wtloginXchg)
            {
                var sigInfo = GetComponent<SigInfoMan>();
                {
                    switch (wtloginXchg.Type)
                    {
                        case EventWtLoginExchange.EventType.Tgtgt:
                            return OnSendRequestTgtgt(sigInfo);
                        case EventWtLoginExchange.EventType.RefreshSms:
                            return OnSendRequestRefreshSms(sigInfo);
                        case EventWtLoginExchange.EventType.CheckSliderCaptcha:
                            return OnSendRequestCheckSlider(sigInfo, wtloginXchg.CaptchaResult);
                        case EventWtLoginExchange.EventType.CheckSmsCaptcha:
                            return OnSendRequestCheckSms(sigInfo, wtloginXchg.CaptchaResult);

                        default:
                            return EventParacel.Reject;
                    }
                }
            }
            else if (eventParacel is EventSsoMessage ssoEvent)
            {
                var sigInfo = GetComponent<SigInfoMan>();
                var oicqRequest = new OicqRequest(ssoEvent.PayloadMsg.GetPayload(), sigInfo.ShareKey);

                Console.WriteLine($"  [oicqRequest] oicqCommand => {oicqRequest.oicqCommand}");
                Console.WriteLine($"  [oicqRequest] oicqVersion => {oicqRequest.oicqVersion}");
                Console.WriteLine($"  [oicqRequest] oicqStatus => {oicqRequest.oicqStatus}");

                PostEvent<SigInfoMan>(new EventUpdateSigInfo
                {
                    WtLoginOicqStatus = oicqRequest.oicqStatus
                });

                switch (oicqRequest.oicqStatus)
                {
                    case OicqStatus.OK:
                        return OnRecvResponseWtloginSuccess(sigInfo, oicqRequest);

                    case OicqStatus.DoVerifySliderCaptcha:
                        return OnRecvResponseVerifySliderCaptcha(sigInfo, oicqRequest);
                    case OicqStatus.DoVerifyDeviceLockViaSms:
                        return OnRecvResponseVerifySmsCaptcha(sigInfo, oicqRequest);

                    case OicqStatus.PreventByIncorrectUserOrPwd:
                        return OnRecvResponseInvalidUsrPwd(sigInfo, oicqRequest);
                    case OicqStatus.PreventByIncorrectSmsCode:
                        return OnRecvResponseInvalidSmsCode(sigInfo, oicqRequest);
                    case OicqStatus.PreventByInvalidEnvironment:
                        return OnRecvResponseInvalidLoginEnv(sigInfo, oicqRequest);
                    case OicqStatus.PreventByLoginDenied:
                        return OnRecvResponseLoginDenied(sigInfo, oicqRequest);

                    default:
                        return OnRecvResponseUnknown(sigInfo, oicqRequest);
                }
            }

            return EventParacel.Reject;
        }

        #region Event Requests

        private EventParacel OnSendRequestTgtgt(SigInfoMan sigInfo)
        {
            Console.WriteLine("Submit OicqRequestTGTGT.");

            return CallEvent<SsoMan>(new EventDraftSsoMessage
            {
                EventDelegate = (EventParacel eventParacel) =>
                {
                    if (eventParacel is EventDraftSsoMessage sso)
                        return new EventSsoMessage
                        {
                            Command = ServiceName,
                            RequestFlag = RequestFlag.WtLoginExchange,
                            PayloadMsg = new SsoMessageTypeA(ServiceName, sso.Sequence, sso.Session, null,
                                new OicqRequestTgtgt(sigInfo, sso.Sequence))
                        };

                    return EventParacel.Reject;
                }
            });
        }

        private EventParacel OnSendRequestCheckSlider(SigInfoMan sigInfo, string ticket)
        {
            Console.WriteLine("Submit OicqRequestCheckImage.");

            return CallEvent<SsoMan>(new EventDraftSsoMessage
            {
                EventDelegate = (EventParacel eventParacel) =>
                {
                    if (eventParacel is EventDraftSsoMessage sso)
                        return new EventSsoMessage
                        {
                            Command = ServiceName,
                            RequestFlag = RequestFlag.WtLoginExchange,
                            PayloadMsg = new SsoMessageTypeA(ServiceName, sso.Sequence, sso.Session, null,
                                new OicqRequestCheckImage(sigInfo, ticket))
                        };

                    return EventParacel.Reject;
                }
            });
        }

        private EventParacel OnSendRequestCheckSms(SigInfoMan sigInfo, string smsCode)
        {
            Console.WriteLine("Submit OicqRequestCheckSms.");

            return CallEvent<SsoMan>(new EventDraftSsoMessage
            {
                EventDelegate = (EventParacel eventParacel) =>
                {
                    if (eventParacel is EventDraftSsoMessage sso)
                        return new EventSsoMessage
                        {
                            Command = ServiceName,
                            RequestFlag = RequestFlag.WtLoginExchange,
                            PayloadMsg = new SsoMessageTypeA(ServiceName, sso.Sequence, sso.Session, null,
                                new OicqRequestCheckSms(sigInfo, smsCode))
                        };

                    return EventParacel.Reject;
                }
            });
        }

        private EventParacel OnSendRequestRefreshSms(SigInfoMan sigInfo)
        {
            Console.WriteLine("Request send SMS.");

            return CallEvent<SsoMan>(new EventDraftSsoMessage
            {
                EventDelegate = (EventParacel eventParacel) =>
                {
                    if (eventParacel is EventDraftSsoMessage sso)
                        return new EventSsoMessage
                        {
                            Command = ServiceName,
                            RequestFlag = RequestFlag.WtLoginExchange,
                            PayloadMsg = new SsoMessageTypeA(ServiceName, sso.Sequence, sso.Session, null,
                                new OicqRequestRefreshSms(sigInfo))
                        };

                    return EventParacel.Reject;
                }
            });
        }

        #endregion

        #region Event Handlers

        private EventParacel OnRecvResponseVerifySliderCaptcha(SigInfoMan sigInfo, OicqRequest request)
        {
            Console.WriteLine("Do slider verification.");

            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv104 = unpacker.TryGetTlv(0x104);
            Tlv tlv192 = unpacker.TryGetTlv(0x192);
            if (tlv104 != null && tlv192 != null)
            {
                var sigSession = ((T104Body)tlv104._tlvBody)._sigSession;
                var sigCaptchaURL = ((T192Body)tlv192._tlvBody)._url;

                CallEvent<SigInfoMan>(new EventUpdateSigInfo
                {
                    WtLoginSession = sigSession
                });

                PostEvent<ToUser>(new EventWtLoginExchange
                {
                    SliderUrl = sigCaptchaURL,
                    Type = EventWtLoginExchange.EventType.CheckSliderCaptcha
                });

                return EventParacel.Accept;
            }

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseVerifySmsCaptcha(SigInfoMan sigInfo, OicqRequest request)
        {
            Console.WriteLine("Do sms verification.");

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

                    Console.WriteLine($"[Hint] {sigMessage}");

                    CallEvent<SigInfoMan>(new EventUpdateSigInfo
                    {
                        WtLoginSession = sigSession,
                        WtLoginSmsPhone = smsPhone,
                        WtLoginSmsToken = smsToken,
                        WtLoginSmsCountry = smsCountryCode
                    });

                    PostEvent<Login>(new EventWtLoginExchange
                    {
                        Type = EventWtLoginExchange.EventType.RefreshSms
                    });

                    return EventParacel.Accept;
                }
            }
            else if (unpacker.Count == 2)
            {
                Tlv tlv104 = unpacker.TryGetTlv(0x104);
                Tlv tlv17b = unpacker.TryGetTlv(0x17b);

                if (tlv104 != null && tlv17b != null)
                {
                    var sigSession = ((T104Body)tlv104._tlvBody)._sigSession;

                    CallEvent<SigInfoMan>(new EventUpdateSigInfo
                    {
                        WtLoginSession = sigSession,
                    });

                    PostEvent<ToUser>(new EventWtLoginExchange
                    {
                        SmsPhoneNumber = sigInfo.WtLoginSmsPhone,
                        SmsPhoneCountryCode = sigInfo.WtLoginSmsCountry,
                        Type = EventWtLoginExchange.EventType.CheckSmsCaptcha
                    });

                    return EventParacel.Accept;
                }
            }
            else
            {
                BroadcastEvent(new EventOnlineStatus
                {
                    Type = EventOnlineStatus.EventType.Offline,
                    Reason = "Unknown data received."
                });
            }

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseVerifyImageCaptcha(SigInfoMan sigInfo, OicqRequest request)
        {
            // <TODO> Image captcha

            BroadcastEvent(new EventOnlineStatus
            {
                Type = EventOnlineStatus.EventType.Offline,
                Reason = "Image captcha not implemented."
            });

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseVerifyDeviceLock(SigInfoMan sigInfo, OicqRequest request)
        {
            // <TODO> Device lock

            BroadcastEvent(new EventOnlineStatus
            {
                Type = EventOnlineStatus.EventType.Offline,
                Reason = "DeviceLock not implemented."
            });

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseWtloginSuccess(SigInfoMan sigInfo, OicqRequest request)
        {
            Console.WriteLine("Wtlogin success.");

            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            if (unpacker.Count == 2)
            {
                Tlv tlv119 = unpacker.TryGetTlv(0x119);
                Tlv tlv161 = unpacker.TryGetTlv(0x161);

                if (tlv119 != null && tlv161 != null)
                {
                    var decrtpted = tlv119._tlvBody.TakeDecryptedBytes(out var _,
                        TeaCryptor.Instance, sigInfo.TgtgKey);

                    var tlv119Unpacker = new TlvUnpacker(decrtpted, true);

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

                    PostEvent<SsoMan>(new EventDropServiceSsoSeq
                    {
                        ServiceName = ServiceName
                    });

                    BroadcastEvent(new EventUpdateSigInfo
                    {
                        TgtKey = tgtKey,
                        TgtToken = tgtToken,

                        D2Key = d2Key,
                        D2Token = d2Token,

                        GtKey = gtKey,
                        StKey = stKey,

                        WtSessionTicketSig = wtSessionTicketSig,
                        WtSessionTicketKey = wtSessionTicketKey,

                        UinInfo = new EventUpdateSigInfo.Info
                        {
                            Age = userAge,
                            Face = userFace,
                            Name = userNickname
                        }
                    });

                    Console.WriteLine($"gtKey => {Hex.Bytes2HexStr(gtKey)}");
                    Console.WriteLine($"stKey => {Hex.Bytes2HexStr(stKey)}");
                    Console.WriteLine($"tgtKey => {Hex.Bytes2HexStr(tgtKey)}");
                    Console.WriteLine($"tgtToken => {Hex.Bytes2HexStr(tgtToken)}");
                    Console.WriteLine($"d2Key => {Hex.Bytes2HexStr(d2Key)}");
                    Console.WriteLine($"d2Token => {Hex.Bytes2HexStr(d2Token)}");

                    return EventParacel.Accept;
                }
            }

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseInvalidUsrPwd(SigInfoMan sigInfo, OicqRequest request)
        {
            BroadcastEvent(new EventOnlineStatus
            {
                Type = EventOnlineStatus.EventType.Offline,
                Reason = "Incorrect account or password."
            });

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseInvalidSmsCode(SigInfoMan sigInfo, OicqRequest request)
        {
            BroadcastEvent(new EventOnlineStatus
            {
                Type = EventOnlineStatus.EventType.Offline,
                Reason = "Incorrect sms code."
            });

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseInvalidLoginEnv(SigInfoMan sigInfo, OicqRequest request)
        {
            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv146 = unpacker.TryGetTlv(0x146);
            if (tlv146 != null)
            {
                var errorTitle = ((T146Body)tlv146._tlvBody)._title;
                var errorMessage = ((T146Body)tlv146._tlvBody)._message;

                Console.WriteLine($"[Error] {errorTitle} {errorMessage}");
            }

            BroadcastEvent(new EventOnlineStatus
            {
                Type = EventOnlineStatus.EventType.Offline,
                Reason = "Invalid login environment."
            });

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseLoginDenied(SigInfoMan sigInfo, OicqRequest request)
        {
            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv146 = unpacker.TryGetTlv(0x146);
            if (tlv146 != null)
            {
                var errorTitle = ((T146Body)tlv146._tlvBody)._title;
                var errorMessage = ((T146Body)tlv146._tlvBody)._message;

                Console.WriteLine($"[Error] {errorTitle} {errorMessage}");
            }

            BroadcastEvent(new EventOnlineStatus
            {
                Type = EventOnlineStatus.EventType.Offline,
                Reason = "Login denied."
            });

            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponseUnknown(SigInfoMan sigInfo, OicqRequest request)
        {
            BroadcastEvent(new EventOnlineStatus
            {
                Type = EventOnlineStatus.EventType.Offline,
                Reason = "Unknown OicqRequest received."
            });

            return EventParacel.Reject;
        }

        #endregion
    }
}
