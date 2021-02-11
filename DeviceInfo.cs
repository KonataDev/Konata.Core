using System;
using System.Linq;
using System.Text;

using Konata.Utils.Crypto;

namespace Konata.Core
{
    public enum NetworkType
    {
        Other = 0,
        Mobile = 1,
        Wifi = 2,
    }

    public static class DeviceInfo
    {
        public static byte[] Guid
        {
            get
            {
                return new Md5Cryptor().Encrypt(Encoding.UTF8.GetBytes(System.AndroidId).Concat(Network.Wifi.IpAddress).ToArray());
            }
        }

        public static class System
        {
            public static string Os = "android";
            public static string OsVersion = "8.1.0";
            public static string OsName = "klteduosctc-user 6.0";
            public static string AndroidId = "8edadfb1e4a02cdc";

            public static string ModelName = "SM-G9009W";
            public static string Manufacturer = "samsung";

            public static string Imei
            {
                get
                {
                    return "735273365111018";
                }
            }

            public static string Imsi
            {
                get
                {
                    return "123123333123123";
                }
            }
        }


        public static string BootId
        {
            get
            {
                return new Guid().ToString();
            }
        }

        public static class Build
        {

            public static string Bootloader
            {
                get
                {
                    return "G9009WKEU1BOL1";
                }
            }

            public static string CodeName
            {
                get
                {
                    return "REL";
                }
            }

            public static string BaseBand
            {
                get
                {
                    return "G9009WKEU1BOL1";
                }
            }

            public static string Incremental
            {
                get
                {
                    return "c0ab6bb259";
                }
            }

            public static string Model
            {
                get
                {
                    return "klte";
                }
            }

            public static string InnerVersion
            {
                get
                {
                    var version = "lineage_kltechnduo-userdebug 8.1.0 OPM2.171026.006.H1 c0ab6bb259";
                    return version ?? Incremental;
                }
            }

            public static string Fingerprint
            {
                get
                {
                    return "samsung/klteduosctc/klte:6.0.1/MMB29M/G9009WKEU1CQB2:user/release-keys";
                }
            }

        }

        public static class Network
        {

            public static NetworkType Type = NetworkType.Wifi;

            public static class Wifi
            {

                public static byte[] IpAddress
                {
                    get
                    {
                        return new byte[] { 0, 0, 0, 0 };
                    }

                }

                public static byte[] MacAddress
                {
                    get
                    {
                        return new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };
                    }
                }

                public static byte[] ApMacAddress
                {
                    get
                    {
                        return new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };
                    }
                }

                public static string ApnName
                {

                    get
                    {
                        return "wifi";
                    }
                }

                public static string Ssid
                {
                    get
                    {
                        return "<hidden SSID>";
                    }
                }

            }

            public static class Mobile
            {
                public static string OperatorName
                {
                    get
                    {
                        return "China Telecom";
                    }
                }

                public static string ApnName
                {

                    get
                    {
                        return "CTNET"; // CTNET / CTWAP / CTIMS
                    }
                }
            }

        }

        public static class Display
        {
            public static int Width
            {
                get => 504;
            }

            public static int Height
            {
                get => 896;
            }
        }
    }
}
