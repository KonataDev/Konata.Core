using System;
using Konata.Core.Utils;
using Guid = Konata.Core.Utils.Guid;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Common;

/// <summary>
/// Bot device definitions
/// </summary>
public class BotDevice
{
    /// <summary>
    /// Device model information
    /// </summary>
    public ModelInfo Model { get; set; }

    /// <summary>
    /// Display information
    /// </summary>
    public DisplayInfo Display { get; set; }

    /// <summary>
    /// System information
    /// </summary>
    public SystemInfo System { get; set; }

    /// <summary>
    /// Network information
    /// </summary>
    public NetworkInfo Network { get; set; }

    /// <summary>
    /// Model
    /// </summary>
    public class ModelInfo
    {
        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Device manufacturer
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Device Imei
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// Device Imsi
        /// </summary>
        public string Imsi { get; set; }

        /// <summary>
        /// Device baseband
        /// </summary>
        public string BaseBand { get; set; }

        /// <summary>
        /// Device codename
        /// </summary>
        public string CodeName { get; set; }
    }

    /// <summary>
    /// System info
    /// </summary>
    public class SystemInfo
    {
        public string Name { get; set; }

        public string Type => "android";

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
        var randName = $"Andreal {StringGen.GetRandHex(8)}";
        var randBaseBand = $"ANDREAL.CORE.{StringGen.GetRandHex(4)}.1.0.REV";
        var randImei = StringGen.GetRandNumber(14);
        var randImsi = StringGen.GetRandNumber(15);

        // Roll a random system
        var randOsVer = RandOsVer();
        var randGuid = Guid.GenerateBytes();
        var randBootId = StringGen.GetRandHex(16, false);
        var randBootLoader = $"KONATA.CORE.{StringGen.GetRandHex(8)}.BOOTLOADER_1_0";
        var randAndroidId = StringGen.GetRandHex(16, false);
        var randIncremental = StringGen.GetRandHex(16, false);
        var randInnerVersion = $"KONATA.CORE.USERDEBUG.{randOsVer}";
        var randFringerPrint = $"konata/core/konata:{randOsVer}/" +
                               $"{StringGen.GetRandNumber(8)}:user/release-keys";

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
                Manufacturer = "Project Andreal",
                BaseBand = randBaseBand,
                Imei = randImei,
                Imsi = randImsi
            },

            System = new()
            {
                Name = "konata",
                Version = randOsVer,
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
        return $"{rand.Next(7, 12)}." +
               $"{rand.Next(0, 2)}." +
               $"{rand.Next(0, 2)}";
    }
}
