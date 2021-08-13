using System;
using Konata.Core.Utils;
using Guid = System.Guid;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeMadeStatic.Global

namespace Konata.Core
{
    public class BotDevice
    {
        public ModelInfo Model { get; set; }

        public DisplayInfo Display { get; set; }

        public SystemInfo System { get; set; }

        public NetworkInfo Network { get; set; }

        public class ModelInfo
        {
            public string Name { get; set; }

            public string Manufacturer { get; set; }

            public string IMEI { get; set; }

            public string IMSI { get; set; }

            public string BaseBand { get; set; }

            public string CodeName { get; set; }
        }

        public class SystemInfo
        {
            public string Name { get; set; }

            public string Type
            {
                get => "android";
            }

            public string Version { get; set; }

            public string BootId { get; set; }

            public string BootLoader { get; set; }

            public string AndroidId { get; set; }

            public string Incremental { get; set; }

            public string InnerVersion { get; set; }

            public string FingerPrint { get; set; }

            public byte[] Guid { get; set; }
        }

        public class NetworkInfo
        {
            public NetworkType NetType { get; set; }

            [Obsolete] public string NetApn { get; set; }

            public string NetOperator { get; set; }

            [Obsolete] public byte[] NetIpAddress { get; set; }

            public byte[] WifiMacAddress { get; set; }

            public string WifiSsid { get; set; }

            public byte[] WifiBssid { get; set; }
        }

        public class DisplayInfo
        {
            public int Width { get; set; }

            public int Height { get; set; }
        }

        public enum NetworkType
        {
            Other = 0,
            Mobile = 1,
            Wifi = 2,
        }

        /// <summary>
        /// Get a random device
        /// </summary>
        /// <returns></returns>
        public static BotDevice Default()
        {
            // Roll a random model
            var randName = $"Konata {Strings.GetRandHex(4)}";
            var randBaseBand = $"KONATA.CORE.{Strings.GetRandHex(4)}.1.0.REV";
            var randImei = Strings.GetRandNumber(14);
            var randImsi = Strings.GetRandNumber(15);

            // Roll a random system
            var randOSVer = RandOsVer();
            var randGuid = Guid.NewGuid().ToByteArray();
            var randBootId = Strings.GetRandHex(16, false);
            var randBootLoader = $"KONATA.CORE.{Strings.GetRandHex(8)}.BOOTLOADER_1_0";
            var randAndroidId = Strings.GetRandHex(16, false);
            var randIncremental = Strings.GetRandHex(16, false);
            var randInnerVersion = $"KONATA.CORE.USERDEBUG.{randOSVer}";
            var randFringerPrint = $"konata/core/konata:{randOSVer}/" +
                                   $"{Strings.GetRandNumber(8)}:user/release-keys";

            return new BotDevice
            {
                Display = new()
                {
                    Width = 1080,
                    Height = 1920
                },

                Model = new()
                {
                    Name = randName,
                    CodeName = "REL",
                    Manufacturer = "Konata Project",
                    BaseBand = randBaseBand,
                    IMEI = randImei,
                    IMSI = randImsi
                },

                System = new()
                {
                    Name = "konata",
                    Version = randOSVer,
                    BootId = randBootId,
                    BootLoader = randBootLoader,
                    AndroidId = randAndroidId,
                    Incremental = randIncremental,
                    InnerVersion = randInnerVersion,
                    FingerPrint = randFringerPrint,
                    Guid = randGuid
                },

                Network = new()
                {
                    NetApn = "CMNET",
                    NetOperator = "China Mobile",
                    NetIpAddress = new byte[] {0x00, 0x00, 0x00, 0x00},
                    NetType = NetworkType.Wifi,
                    WifiSsid = "<Hidden SSID>",
                    WifiBssid = new byte[] {0xAA, 0xBB, 0xCC, 0xDD, 0xEE},
                    WifiMacAddress = new byte[] {0xAA, 0xBB, 0xCC, 0xDD, 0xEE},
                }
            };
        }

        private static string RandOsVer()
        {
            var rand = new Random();
            return $"{rand.Next(7, 11)}." +
                   $"{rand.Next(0, 2)}." +
                   $"{rand.Next(0, 2)}";
        }
    }
}
