namespace Konata.Core.Packets;

internal static class AppInfo
{
    public const uint AppId = 16;

    public const uint SubAppId = 537113159;

    public const uint AppClientVersion = 0;

    public const string ApkPackageName = "com.tencent.mobileqq";

    public const string ApkVersionName = "8.8.80";

    public const string AppBuildVer = "8.8.80.7400";

    public const string AppRevision = "61cd5c8c";

    public static byte[] ApkSignatureMd5 { get; } =
    {
        0xA6, 0xB7, 0x45, 0xBF,
        0x24, 0xA2, 0xC2, 0x77,
        0x52, 0x77, 0x16, 0xF6,
        0xF3, 0x6E, 0xB6, 0x8D
    };

    public static class WtLoginSdk
    {
        public const uint SdkBuildTime = 1640921786;

        public const string SdkVersion = "6.0.0.2494";

        public const uint MainSigBitmap = 16724722;

        public const uint MiscBitmap = 150470524;

        public const uint SubSigBitmap = 66560;

        public static readonly uint[] SubAppIdList = {1600000226};
    }
}
