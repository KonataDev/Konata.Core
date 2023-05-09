using System;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Tlv;
using Konata.Core.Packets.Tlv.Model;
using Konata.Core.Packets.Oicq.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Packets.Oicq;
using Konata.Core.Utils.Crypto;

// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedVariable
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace Konata.Core.Components.Services.WtLogin;

[EventSubscribe(typeof(WtLoginEvent))]
[Service("wtlogin.login", PacketType.TypeA, AuthFlag.WtLoginExchange, SequenceMode.Session)]
internal class Login : BaseService<WtLoginEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out WtLoginEvent output)
    {
        // Parse oicq response
        var oicqResponse = new OicqResponse(input.Payload.GetBytes(), keystore.Ecdh);

        // Select status
        output = oicqResponse.Status switch
        {
            OicqStatus.OK => OnRecvWtloginSuccess(oicqResponse, keystore),
            OicqStatus.DoVerifyDeviceLock => OnRecvVerifyDeviceLock(oicqResponse, keystore),
            OicqStatus.DoVerifySliderCaptcha => OnRecvCheckSliderCaptcha(oicqResponse, keystore),
            OicqStatus.DoVerifySmsCaptcha => OnRecvCheckSmsCaptcha(oicqResponse, keystore),
            OicqStatus.PreventByIncorrectPassword => OnRecvIncorrectPassword(oicqResponse, keystore),
            OicqStatus.PreventByIncorrectSmsCode => OnRecvIncorrectSmsCode(oicqResponse, keystore),
            OicqStatus.PreventByHighRiskOfEnvironment => OnRecvHighRiskOfEnvironment(oicqResponse, keystore),
            OicqStatus.PreventByOutdatedVersion => OnRecvOutdatedVersion(oicqResponse, keystore),
            OicqStatus.PreventByLoginDenied => OnRecvLoginDenied(oicqResponse, keystore),
            _ => OnRecvUnknown(oicqResponse)
        };

        return true;
    }

    protected override bool Build(int sequence, WtLoginEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        // newSequence = sequence.GetSessionSequence("wtlogin.login");

        // Build OicqRequest
        switch (input.EventType)
        {
            case WtLoginEvent.Type.Tgtgt:
                output = new OicqRequestTgtgt(sequence, appInfo, keystore, device);
                break;

            case WtLoginEvent.Type.CheckSms:
                output = new OicqRequestCheckSms(input.CaptchaResult, appInfo, keystore, device);
                break;

            case WtLoginEvent.Type.RefreshSms:
                output = new OicqRequestRefreshSms(appInfo, keystore);
                break;

            case WtLoginEvent.Type.CheckSlider:
                output = new OicqRequestCheckSlider(input.CaptchaResult, appInfo, keystore, device);
                break;

            case WtLoginEvent.Type.VerifyDeviceLock:
                output = new OicqRequestVerifyDeviceLock(appInfo, keystore);
                break;

            default: return false;
        }

        return true;
    }

    #region Event Handlers

    /// <summary>
    /// Check slider captcha 
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvCheckSliderCaptcha(OicqResponse response, BotKeyStore keystore)
    {
        var tlvs = response.BodyData.TakeAllBytes(out _);
        var unpacker = new TlvUnpacker(tlvs, true);

        Tlv tlv104 = unpacker.TryGetTlv(0x104);
        Tlv tlv192 = unpacker.TryGetTlv(0x192);
        Tlv tlv546 = unpacker.TryGetTlv(0x546);

        if (tlv104 != null && tlv192 != null)
        {
            var sigSession = ((T104Body) tlv104._tlvBody)._sigSession;
            var sigCaptchaURL = ((T192Body) tlv192._tlvBody)._url;

            // Generate tlv547 for submiting captcha
            if (tlv546 != null)
                keystore.Session.WtSessionT547 = new T547Body((T546Body) tlv546._tlvBody).GetBytes();

            // Save wtlogin session id
            keystore.Session.WtLoginSession = sigSession;

            return WtLoginEvent.ResultCheckSlider((int) response.Status, sigCaptchaURL);
        }

        return OnRecvUnknown(response);
    }

    /// <summary>
    /// Check sms captcha
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvCheckSmsCaptcha(OicqResponse response, BotKeyStore keystore)
    {
        var tlvs = response.BodyData.TakeAllBytes(out var _);
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

            if (tlv104 != null && tlv174 != null &&
                tlv204 != null && tlv178 != null &&
                tlv17d != null && tlv402 != null &&
                tlv403 != null && tlv17e != null)
            {
                var sigSession = ((T104Body) tlv104._tlvBody)._sigSession;
                var sigMessage = ((T17eBody) tlv17e._tlvBody)._message;
                var smsPhone = ((T178Body) tlv178._tlvBody)._phone;
                var smsCountryCode = ((T178Body) tlv178._tlvBody)._countryCode;
                var smsToken = ((T174Body) tlv174._tlvBody)._smsToken;

                keystore.Session.WtLoginSession = sigSession;
                keystore.Session.WtLoginSmsPhone = smsPhone;
                keystore.Session.WtLoginSmsToken = smsToken;
                keystore.Session.WtLoginSmsCountry = smsCountryCode;

                return WtLoginEvent.ResultRefreshSms((int) response.Status);
            }
        }
        else if (unpacker.Count == 2)
        {
            Tlv tlv104 = unpacker.TryGetTlv(0x104);
            Tlv tlv17b = unpacker.TryGetTlv(0x17b);

            if (tlv104 != null && tlv17b != null)
            {
                var sigSession = ((T104Body) tlv104._tlvBody)._sigSession;
                keystore.Session.WtLoginSession = sigSession;

                return WtLoginEvent.ResultCheckSms((int) response.Status,
                    keystore.Session.WtLoginSmsPhone, keystore.Session.WtLoginSmsCountry);
            }
        }

        return OnRecvUnknown(response);
    }

    /// <summary>
    /// Verify device lock
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvVerifyDeviceLock(OicqResponse response, BotKeyStore keystore)
    {
        var tlvs = response.BodyData.TakeAllBytes(out var _);
        var unpacker = new TlvUnpacker(tlvs, true);

        if (unpacker.Count == 3)
        {
            Tlv tlv104 = unpacker.TryGetTlv(0x104);
            if (tlv104 != null)
            {
                var sigSession = ((T104Body) tlv104._tlvBody)._sigSession;
                keystore.Session.WtLoginSession = sigSession;

                return WtLoginEvent.ResultVerifyDeviceLock((int) response.Status);
            }

            return OnRecvUnknown(response);
        }

        return WtLoginEvent.ResultVerifyDeviceLock((int) response.Status);
    }

    /// <summary>
    /// Wtlogin success
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvWtloginSuccess(OicqResponse response, BotKeyStore keystore)
    {
        var tlvs = response.BodyData.TakeAllBytes(out var _);
        var unpacker = new TlvUnpacker(tlvs, true);

        if (unpacker.Count == 2)
        {
            Tlv tlv119 = unpacker.TryGetTlv(0x119);
            Tlv tlv161 = unpacker.TryGetTlv(0x161);

            if (tlv119 != null && tlv161 != null)
            {
                var decrypted = tlv119._tlvBody.TakeDecryptedBytes(out var _,
                    TeaCryptor.Instance, keystore.KeyStub.TgtgKey);

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
                keystore.Account.Age = userAge;
                keystore.Account.Face = userFace;
                keystore.Account.Name = userNickname;
                keystore.Account.Age = userAge;

                return WtLoginEvent.ResultOk((int) response.Status);
            }
        }

        return OnRecvUnknown(response);
    }

    /// <summary>
    /// Incorrect user or password
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvIncorrectPassword(OicqResponse response, BotKeyStore keystore)
        => WtLoginEvent.ResultInvalidUsrPwd((int) response.Status);

    /// <summary>
    /// Incorrect sms code
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvIncorrectSmsCode(OicqResponse response, BotKeyStore keystore)
        => WtLoginEvent.ResultInvalidSmsCode((int) response.Status);

    /// <summary>
    /// High risk of environment
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvHighRiskOfEnvironment(OicqResponse response, BotKeyStore keystore)
        => OnRecvMessageTips(WtLoginEvent.Type.HighRiskOfEnvironment, response, keystore);

    /// <summary>
    /// Outdated version
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvOutdatedVersion(OicqResponse response, BotKeyStore keystore)
        => OnRecvMessageTips(WtLoginEvent.Type.OutdatedVersion, response, keystore);

    /// <summary>
    /// Any error denied login
    /// </summary>
    /// <param name="response"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    private WtLoginEvent OnRecvLoginDenied(OicqResponse response, BotKeyStore keystore)
        => OnRecvMessageTips(WtLoginEvent.Type.LoginDenied, response, keystore);

    /// <summary>
    /// Unknown code
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    private static WtLoginEvent OnRecvUnknown(OicqResponse response)
        => WtLoginEvent.ResultUnknown((int) response.Status, "Unknown OicqRequest received.");

    private static WtLoginEvent OnRecvMessageTips
        (WtLoginEvent.Type type, OicqResponse response, BotKeyStore keystore)
    {
        var tlvs = response.BodyData.TakeAllBytes(out var _);
        var unpacker = new TlvUnpacker(tlvs, true);

        Tlv tlv146 = unpacker.TryGetTlv(0x146);
        if (tlv146 != null)
        {
            var errorTitle = ((T146Body) tlv146._tlvBody)._title;
            var errorMessage = ((T146Body) tlv146._tlvBody)._message;
            var errorInformation = ((T146Body) tlv146._tlvBody)._errorInfo;

            return WtLoginEvent.Result(type, (int) response.Status,
                $"{errorTitle} {errorMessage} {errorInformation}");
        }

        return OnRecvUnknown(response);
    }

    #endregion
}
