using System;

namespace Konata.Core.Packet
{
    public enum LoginType
    {
        Password = 1,
        Sms = 3,
        WeChat = 4
    }

    public enum WtLoginSigType : uint
    {
        WLOGIN_A5 = 2,
        WLOGIN_RESERVED = 16, // A8
        WLOGIN_STWEB = 32,
        WLOGIN_A2 = 64,
        WLOGIN_ST = 128,
        WLOGIN_LSKEY = 512,
        WLOGIN_SKEY = 4096,
        WLOGIN_SIG64 = 8192,
        WLOGIN_OPENKEY = 16384,
        WLOGIN_TOKEN = 32768,
        WLOGIN_VKEY = 131072,
        WLOGIN_D2 = 262144,
        WLOGIN_SID = 524288,
        WLOGIN_PSKEY = 1048576,
        WLOGIN_AQSIG = 2097152,
        WLOGIN_LHSIG = 4194304,
        WLOGIN_PAYTOKEN = 8388608,
        WLOGIN_PF = 16777216,
        WLOGIN_DA2 = 33554432,
        WLOGIN_QRPUSH = 67108864,
        WLOGIN_PT4Token = 134217728,
    }
}
