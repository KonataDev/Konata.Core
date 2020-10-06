using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata
{
    public static class AppInfo
    {
        public static readonly uint appId = 16;
        public static readonly uint subAppId = 537063202;
        public static readonly uint appClientVersion = 0;
        public static readonly string apkPackageName = "com.tencent.mobileqq";
        public static readonly string apkVersionName = "8.2.7";
        public static readonly string appBuildVer = "8.2.7.4395";
        public static readonly string appRevision = "7288ad61";

        public static readonly uint wtLoginMiscBitmap = 150470524;
        public static readonly uint wtLoginSubSigBitmap = 66560;
        public static readonly uint wtLoginMainSigBitmap = 16724722;
        public static readonly uint[] wtLoginSubAppIdList = { 1600000226 };

        public static readonly byte[] apkSignatureMd5 = new byte[]
        {
            0xA6, 0xB7, 0x45, 0xBF,
            0x24, 0xA2, 0xC2, 0x77,
            0x52, 0x77, 0x16, 0xF6,
            0xF3, 0x6E, 0xB6, 0x8D
        };

        public static readonly byte[] apkSignatureSha1 = new byte[]
        {
            0x5A, 0xC2, 0xD1, 0x00, 
            0x2F, 0x2B, 0xBA, 0xC8, 
            0x7C, 0xA0, 0xEC, 0x10, 
            0x02, 0x5C, 0x6C, 0xA0, 
            0x3C, 0xF5, 0x87, 0x4D
        };

        public static class WtLoginSdk
        {
            public static readonly uint buildTime = 1577331209;
            public static readonly string sdkVersion = "6.0.0.2425";
        }



    }
}
