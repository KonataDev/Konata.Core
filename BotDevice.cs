using System;
using System.Linq;
using System.Text;

using Konata.Utils.Crypto;

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

            public string Type { get => "android"; }

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

            [Obsolete]
            public string NetApn { get; set; }

            public string NetOperator { get; set; }

            [Obsolete]
            public byte[] NetIpAddress { get; set; }

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
    }
}
