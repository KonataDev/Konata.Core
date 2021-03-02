using System;
using System.Linq;
using System.Text;

using Konata.Utils.Crypto;

namespace Konata.Core
{
    public class BotDevice
    {
        public ModelInfo Model { get; set; }

        public SystemInfo System { get; set; }

        public NetworkInfo Network { get; set; }

        public byte[] Guid { get; set; }

        //public BotDevice(ModelInfo model,
        //    SystemInfo system, NetworkInfo network)
        //{
        //    Model = model;
        //    System = system;
        //    Network = network;

        //    Guid = new Md5Cryptor()
        //        .Encrypt(Encoding.UTF8.GetBytes(System.AndroidId)
        //        .Concat(network.Wifi.IpAddress)
        //        .ToArray());
        //}

        public struct ModelInfo
        {
            public string Name { get; set; }

            public string Manufacturer { get; set; }

            public string IMEI { get; set; }

            public string IMSI { get; set; }

            public string BaseBand { get; set; }

            public string CodeName { get; set; }

            public DisplayInfo Display { get; set; }
        }

        public struct SystemInfo
        {
            public string Name { get; set; }

            public string Type { get => "android"; }

            public string Version { get; set; }

            public string BootId { get; set; }

            public string BootLoader { get; set; }

            public string AndroidId { get; set; }

            public string Incremental { get; set; }

            public string InnerVersion { get; set; }

            public string FingerPrint { get; set; }
        }

        public struct NetworkInfo
        {
            public NetworkType Type { get; set; }

            public WifiInfo Wifi { get; set; }

            public MobileInfo Mobile { get; set; }
        }

        public enum NetworkType
        {
            Other = 0,
            Mobile = 1,
            Wifi = 2,
        }

        public struct WifiInfo
        {
            [Obsolete]
            public string Apn { get; set; }

            [Obsolete]
            public byte[] IpAddress { get; set; }

            public byte[] MacAddress { get; set; }

            public string Ssid { get; set; }

            public byte[] Bssid { get; set; }
        }

        public struct MobileInfo
        {
            public string Apn { get; set; }

            public string Operator { get; set; }
        }

        public struct DisplayInfo
        {
            public int Width { get; set; }

            public int Height { get; set; }
        }
    }
}
