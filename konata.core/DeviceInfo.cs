using System;
using System.Linq;
using System.Text;
using Konata.Protocol;
using Konata.Utils.Crypto;

namespace Konata
{
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
            public static string AndroidId = "9774d56d682e549c";

            public static string ModelName = "SM-G9009W";
            public static string Manufacturer = "samsung";

            public static string Imei
            {
                get
                {
                    return "12";
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
                    return "Bootloader";
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
                    return "samsung";
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
                    return Incremental;
                }
            }

            public static string Fingerprint
            {
                get
                {
                    return "76a92151209405be1ec0858a74afd431";
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
                        return "ChinaTelecom";
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

    }

}
