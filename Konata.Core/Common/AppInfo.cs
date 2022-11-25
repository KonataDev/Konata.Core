namespace Konata.Core.Common;

internal class AppInfo
{
    public uint AppId { get; private set; }
    public uint SubAppId { get; private set; }
    public uint AppClientVersion { get; private set; }
    public string ApkPackageName { get; private set; }
    public string ApkVersionName { get; private set; }
    public string AppBuildVer { get; private set; }
    public string AppRevision { get; private set; }
    public byte[] ApkSignatureMd5 { get; private set; }
    public WtLoginSdkDef WtLoginSdk { get; private set; }

    internal class WtLoginSdkDef
    {
        public uint SdkBuildTime { get; internal set; }
        public string SdkVersion { get; internal set; }
        public uint MainSigBitmap { get; internal set; }
        public uint MiscBitmap { get; internal set; }
        public uint SubSigBitmap { get; internal set; }
        public uint[] SubAppIdList { get; internal set; }
    }

    public static AppInfo AndroidPhone => new()
    {
        AppId = 16,
        SubAppId = 537138832,
        AppClientVersion = 0,
        ApkPackageName = "com.tencent.mobileqq",
        ApkVersionName = "8.9.15.9425",
        AppBuildVer = "8.8.80.7400",
        AppRevision = "61cd5c8c",
        ApkSignatureMd5 = new byte[]
        {
            0xA6, 0xB7, 0x45, 0xBF,
            0x24, 0xA2, 0xC2, 0x77,
            0x52, 0x77, 0x16, 0xF6,
            0xF3, 0x6E, 0xB6, 0x8D
        },
        WtLoginSdk = new()
        {
            SdkBuildTime = 1640921786,
            SdkVersion = "6.0.0.2494",
            MainSigBitmap = 16724722,
            MiscBitmap = 150470524,
            SubSigBitmap = 66560,
            SubAppIdList = new uint[] {1600000226}
        }
    };

    public static AppInfo AndroidPad => new()
    {
        AppId = 16,
        SubAppId = 537140414,
        AppClientVersion = 0,
        ApkPackageName = "com.tencent.mobileqq",
        ApkVersionName = "8.9.15.9425",
        AppBuildVer = "8.8.80.7400",
        AppRevision = "61cd5c8c",
        ApkSignatureMd5 = new byte[]
        {
            0xA6, 0xB7, 0x45, 0xBF,
            0x24, 0xA2, 0xC2, 0x77,
            0x52, 0x77, 0x16, 0xF6,
            0xF3, 0x6E, 0xB6, 0x8D
        },
        WtLoginSdk = new()
        {
            SdkBuildTime = 1640921786,
            SdkVersion = "6.0.0.2494",
            MainSigBitmap = 16724722,
            MiscBitmap = 150470524,
            SubSigBitmap = 66560,
            SubAppIdList = new uint[] { 1600000226 }
        }
    };

    public static AppInfo Watch => new()
    {
        AppId = 16,
        SubAppId = 537064446,
        AppClientVersion = 0,
        ApkPackageName = "com.tencent.qqlite",
        ApkVersionName = "3.3.0",
        AppBuildVer = "3.3.0.20",
        AppRevision = "", // empty for watch
        ApkSignatureMd5 = new byte[]
        {
            0xA6, 0xB7, 0x45, 0xBF,
            0x24, 0xA2, 0xC2, 0x77,
            0x52, 0x77, 0x16, 0xF6,
            0xF3, 0x6E, 0xB6, 0x8D
        },
        WtLoginSdk = new()
        {
            SdkBuildTime = 1559564731,
            SdkVersion = "6.0.0.2365",
            MainSigBitmap = 16724722,
            MiscBitmap = 16252796,
            SubSigBitmap = 66560,
            SubAppIdList = new uint[] {1600000226}
        }
    };
    
    public static AppInfo Ipad => new()
    {
        AppId = 12,
        SubAppId = 537097188,
        AppClientVersion = 0,
        ApkPackageName = "com.tencent.minihd.qq",
        ApkVersionName = "8.8.35",
        AppBuildVer = "8.8.38.2266",
        AppRevision = "61cd5c8c",
        ApkSignatureMd5 = new byte[]
        {
            0xAA, 0x39, 0x78, 0xF4,
            0x1F, 0xD9, 0x6F, 0xF9,
            0x91, 0x4A, 0x66, 0x9E,
            0x18, 0x64, 0x74, 0xC7
        },
        WtLoginSdk = new()
                    {
                        SdkBuildTime = 1640921786,
                        SdkVersion = "6.0.0.2433",
                        MainSigBitmap = 1970400,
                        MiscBitmap = 150470524,
                        SubSigBitmap = 66560,
                        SubAppIdList = new uint[] { 1600000226 }
                    }
    };
}
