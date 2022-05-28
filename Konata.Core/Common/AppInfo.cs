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
    
    public static AppInfo Android => new()
    {
        AppId = 16,
        SubAppId = 537113159,
        AppClientVersion = 0,
        ApkPackageName = "com.tencent.mobileqq",
        ApkVersionName = "8.8.80",
        AppBuildVer = "8.8.80.7400",
        AppRevision = "61cd5c8c",
        ApkSignatureMd5 = new byte[]
        {
            0xA6, 0xB7, 0x45, 0xBF,
            0x24, 0xA2, 0xC2, 0x77,
            0x52, 0x77, 0x16, 0xF6,
            0xF3, 0x6E, 0xB6, 0x8D
        },
        WtLoginSdk = new WtLoginSdkDef()
        {
            SdkBuildTime = 1640921786,
            SdkVersion = "6.0.0.2494",
            MainSigBitmap = 16724722,
            MiscBitmap = 150470524,
            SubSigBitmap = 66560,
            SubAppIdList = new uint[] {1600000226}
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
        WtLoginSdk = new WtLoginSdkDef()
        {
            SdkBuildTime = 1559564731,
            SdkVersion = "6.0.0.2365",
            MainSigBitmap = 16724722,
            MiscBitmap = 16252796,
            SubSigBitmap = 66560,
            SubAppIdList = new uint[] {1600000226}
        }
    };

}