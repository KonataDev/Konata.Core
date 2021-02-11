using System;

namespace Konata.Core.Packet
{
    internal static class AppInfo
    {
        public static uint AppId { get; } = 16;

        public static uint SubAppId { get; } = 537063202;

        public static uint AppClientVersion { get; } = 0;

        public static string ApkPackageName { get; } = "com.tencent.mobileqq";

        public static string ApkVersionName { get; } = "8.2.7";

        public static string AppBuildVer { get; } = "8.2.7.4395";

        public static string AppRevision { get; } = "7288ad61";

        public static byte[] ApkSignatureMd5 { get; } = new byte[]
        {
            0xA6, 0xB7, 0x45, 0xBF,
            0x24, 0xA2, 0xC2, 0x77,
            0x52, 0x77, 0x16, 0xF6,
            0xF3, 0x6E, 0xB6, 0x8D
        };

        public static byte[] ApkSignatureSha1 { get; } = new byte[]
        {
            0x5A, 0xC2, 0xD1, 0x00,
            0x2F, 0x2B, 0xBA, 0xC8,
            0x7C, 0xA0, 0xEC, 0x10,
            0x02, 0x5C, 0x6C, 0xA0,
            0x3C, 0xF5, 0x87, 0x4D
        };

        public static class WtLoginSdk
        {
            public static uint SdkBuildTime { get; } = 1577331209;

            public static string SdkVersion { get; } = "6.0.0.2425";

            public static uint MainSigBitmap { get; } = 16724722;

            public static uint MiscBitmap { get; } = 150470524;

            public static uint SubSigBitmap { get; } = 66560;

            public static uint[] SubAppIdList { get; } = { 1600000226 };
        }
    }
}
