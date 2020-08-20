using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata
{
    public static class AppInfo
    {
        public const uint appId = 16;
        public const uint subAppId = 537063202;
        public const int appClientVersion = 0;
        public const string apkPackageName = "com.tencent.mobileqq";
        public const string apkVersionName = "8.2.7";
        public const string appBuildVer = "8.2.7.4395";
        public const string appRevision = "7288ad61";

        public static readonly byte[] apkSignature = new byte[]
        {
            0xA6, 0xB7, 0x45, 0xBF,
            0x24, 0xA2, 0xC2, 0x77,
            0x52, 0x77, 0x16, 0xF6,
            0xF3, 0x6E, 0xB6, 0x8D
        };

        public static class WtLoginSdk
        {
            public const uint buildTime = 1577331209;
            public const string sdkVersion = "6.0.0.2425";
        }

    }
}
