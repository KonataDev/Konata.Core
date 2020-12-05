using System;

using Konata.Core.Event;
using Konata.Core.Packet;
using Konata.Core.Packet.Tlv;
using Konata.Core.Packet.Tlv.TlvModel;
using Konata.Core.Packet.Oicq;
using Konata.Core.Manager;

using Konata.Utils;
using Konata.Utils.Crypto;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Service.WtLogin
{
    [SSOService("wtlogin.login", "WtLogin exchange")]
    public class Login : ISSOService
    {
        public bool HandleInComing(EventSsoFrame ssoMessage, out KonataEventArgs output)
        {
            var sigInfo = ssoMessage.Owner.GetComponent<UserSigManager>();
            var oicqRequest = new OicqRequest(ssoMessage.Payload.GetBytes(), sigInfo.ShareKey);

            Console.WriteLine($"  [oicqRequest] oicqCommand => {oicqRequest.oicqCommand}");
            Console.WriteLine($"  [oicqRequest] oicqVersion => {oicqRequest.oicqVersion}");
            Console.WriteLine($"  [oicqRequest] oicqStatus => {oicqRequest.oicqStatus}");

            switch (oicqRequest.oicqStatus)
            {
                case OicqStatus.OK:
                    output = OnRecvWtloginSuccess(oicqRequest, sigInfo.TgtgKey); break;

                case OicqStatus.DoVerifySliderCaptcha:
                    output = OnRecvCheckSliderCaptcha(oicqRequest); break;
                case OicqStatus.DoVerifyDeviceLockViaSms:
                    output = OnRecvCheckSmsCaptcha(oicqRequest); break;

                case OicqStatus.PreventByIncorrectUserOrPwd:
                    output = OnRecvInvalidUsrPwd(oicqRequest); break;
                case OicqStatus.PreventByIncorrectSmsCode:
                    output = OnRecvInvalidSmsCode(oicqRequest); break;
                case OicqStatus.PreventByInvalidEnvironment:
                    output = OnRecvInvalidLoginEnv(oicqRequest); break;
                case OicqStatus.PreventByLoginDenied:
                    output = OnRecvLoginDenied(oicqRequest); break;

                default:
                    output = OnRecvUnknown(); break;
            }

            output.Owner = ssoMessage.Owner;
            return true;
        }

        #region Event Handlers

        private KonataEventArgs OnRecvCheckSliderCaptcha(OicqRequest request)
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

                return new EventWtLogin
                {
                    WtLoginSession = sigSession,
                    WtLoginSliderURL = sigCaptchaURL,
                    EventType = EventWtLogin.Type.CheckSlider
                };
            }

            return OnRecvUnknown();
        }

        private KonataEventArgs OnRecvCheckSmsCaptcha(OicqRequest request)
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

                    return new EventWtLogin
                    {
                        WtLoginSession = sigSession,
                        WtLoginSmsPhone = smsPhone,
                        WtLoginSmsToken = smsToken,
                        WtLoginSmsCountry = smsCountryCode,

                        EventType = EventWtLogin.Type.RefreshSMS,
                        EventMessage = sigMessage
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

                    return new EventWtLogin
                    {
                        WtLoginSession = sigSession,
                        EventType = EventWtLogin.Type.CheckSMS
                    };
                }
            }

            return OnRecvUnknown();
        }

        private KonataEventArgs OnRecvResponseVerifyImageCaptcha(OicqRequest request)
        {
            // <TODO> Image captcha

            return new EventWtLogin
            {
                EventType = EventWtLogin.Type.NotImplemented,
                EventMessage = "Image captcha not implemented."
            };
        }

        private KonataEventArgs OnRecvResponseVerifyDeviceLock(OicqRequest request)
        {
            // <TODO> Device lock

            return new EventWtLogin
            {
                EventType = EventWtLogin.Type.NotImplemented,
                EventMessage = "DeviceLock not implemented. Please turn off your device lock and try again."
            };
        }

        private KonataEventArgs OnRecvWtloginSuccess(OicqRequest request, byte[] tgtgKey)
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
                    var decrypted = tlv119._tlvBody.TakeDecryptedBytes(out var _,
                        TeaCryptor.Instance, tgtgKey);

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

                    Console.WriteLine($"gtKey => {Hex.Bytes2HexStr(gtKey)}");
                    Console.WriteLine($"stKey => {Hex.Bytes2HexStr(stKey)}");
                    Console.WriteLine($"tgtKey => {Hex.Bytes2HexStr(tgtKey)}");
                    Console.WriteLine($"tgtToken => {Hex.Bytes2HexStr(tgtToken)}");
                    Console.WriteLine($"d2Key => {Hex.Bytes2HexStr(d2Key)}");
                    Console.WriteLine($"d2Token => {Hex.Bytes2HexStr(d2Token)}");

                    return new EventWtLogin
                    {
                        TgtKey = tgtKey,
                        TgtToken = tgtToken,

                        D2Key = d2Key,
                        D2Token = d2Token,

                        GtKey = gtKey,
                        StKey = stKey,

                        WtSessionTicketSig = wtSessionTicketSig,
                        WtSessionTicketKey = wtSessionTicketKey,

                        UinInfo = new EventWtLogin.Info
                        {
                            Age = userAge,
                            Face = userFace,
                            Name = userNickname
                        },

                        EventType = EventWtLogin.Type.OK
                    };
                }
            }

            return OnRecvUnknown();
        }

        private KonataEventArgs OnRecvInvalidUsrPwd(OicqRequest request)
        {
            return new EventWtLogin
            {
                EventType = EventWtLogin.Type.InvalidUinOrPassword,
                EventMessage = "Incorrect account or password."
            };
        }

        private KonataEventArgs OnRecvInvalidSmsCode(OicqRequest request)
        {
            return new EventWtLogin
            {
                EventType = EventWtLogin.Type.InvalidSmsCode,
                EventMessage = "Incorrect sms code."
            };
        }

        private KonataEventArgs OnRecvInvalidLoginEnv(OicqRequest request)
        {
            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv146 = unpacker.TryGetTlv(0x146);
            if (tlv146 != null)
            {
                var errorTitle = ((T146Body)tlv146._tlvBody)._title;
                var errorMessage = ((T146Body)tlv146._tlvBody)._message;

                return new EventWtLogin
                {
                    EventType = EventWtLogin.Type.InvalidLoginEnvironment,
                    EventMessage = $"{errorTitle} {errorMessage}"
                };
            }

            return OnRecvUnknown();
        }

        private KonataEventArgs OnRecvLoginDenied(OicqRequest request)
        {
            var tlvs = request.oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv146 = unpacker.TryGetTlv(0x146);
            if (tlv146 != null)
            {
                var errorTitle = ((T146Body)tlv146._tlvBody)._title;
                var errorMessage = ((T146Body)tlv146._tlvBody)._message;

                return new EventWtLogin
                {
                    EventType = EventWtLogin.Type.LoginDenied,
                    EventMessage = $"{errorTitle} {errorMessage}"
                };
            }

            return OnRecvUnknown();
        }

        private KonataEventArgs OnRecvUnknown()
        {
            return new EventWtLogin
            {
                EventType = EventWtLogin.Type.Unknown,
                EventMessage = "Unknown OicqRequest received."
            };
        }

        #endregion

        public bool HandleOutGoing(KonataEventArgs eventArg, out byte[] output)
        {
            output = null;

            if (eventArg is EventWtLogin e)
            {
                var sigManager = e.Owner.GetComponent<UserSigManager>();
                var ssoManager = e.Owner.GetComponent<SsoInfoManager>();
                var configManager = e.Owner.GetComponent<ConfigManager>();

                OicqRequest oicqRequest;
                var oicqKeyRing = new OicqKeyRing
                {
                    tgtgKey = sigManager.TgtgKey,
                    t106Key = sigManager.Tlv106Key,
                    shareKey = sigManager.ShareKey,
                    randKey = sigManager.RandKey,
                    passwordMd5 = sigManager.PasswordMd5,
                    defaultPublicKey = sigManager.DefaultPublicKey,
                };

                // Build OicqRequest
                switch (e.EventType)
                {
                    case EventWtLogin.Type.Tgtgt:
                        oicqRequest = BuildRequestTgtgt(sigManager.Uin, ssoManager.NewSequence,
                            oicqKeyRing, configManager);
                        break;

                    case EventWtLogin.Type.CheckSMS:
                        oicqRequest = BuildRequestCheckSms(sigManager.Uin, sigManager.WtLoginSession,
                            sigManager.WtLoginSmsToken, e.WtLoginCaptchaResult, sigManager.GSecret, oicqKeyRing);
                        break;

                    case EventWtLogin.Type.RefreshSMS:
                        oicqRequest = BuildRequestRefreshSms(sigManager.Uin, sigManager.WtLoginSession,
                           sigManager.WtLoginSmsToken, oicqKeyRing);
                        break;

                    case EventWtLogin.Type.CheckSlider:
                        oicqRequest = BuildRequestCheckSlider(sigManager.Uin,
                            sigManager.WtLoginSession, e.WtLoginCaptchaResult, oicqKeyRing);
                        break;

                    default:
                        return false;
                }

                // Build to service
                if (EventSsoFrame.Create("wtlogin.login", PacketType.TypeA,
                    ssoManager.NewSequence, ssoManager.Session, oicqRequest, out var ssoFrame))
                {
                    if (EventServiceMessage.Create(ssoFrame, AuthFlag.WtLoginExchange,
                        sigManager.Uin, out var toService))
                    {
                        return EventServiceMessage.Build(toService, out output);
                    }
                }
            }

            return false;
        }

        #region Event Builders

        private OicqRequest BuildRequestTgtgt(uint uin, uint ssoSequence,
            OicqKeyRing keyRing, ConfigManager configInfo)
            => new OicqRequestTgtgt(uin, ssoSequence, keyRing);

        private OicqRequest BuildRequestCheckSms(uint uin, string session,
            string smsToken, string smsCode, byte[] gSecret, OicqKeyRing keyRing)
            => new OicqRequestCheckSms(uin, session, smsToken, smsCode, gSecret, keyRing);

        private OicqRequest BuildRequestCheckSlider(uint uin, string session,
            string ticket, OicqKeyRing keyRing)
            => new OicqRequestCheckImage(uin, session, ticket, keyRing);

        private OicqRequest BuildRequestRefreshSms(uint uin, string session,
            string smsToken, OicqKeyRing keyRing)
            => new OicqRequestRefreshSms(uin, session, smsToken, keyRing);

        #endregion
    }
}
