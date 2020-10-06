using System;

namespace Konata.Device.Tree.Samsung
{
    class klte : DeviceInfo
    {
        #region Device Information

        public override string ModelName => "SM-G9009W";

        public override string Manufacturer => "SAMSUNG";

        #endregion

        #region Android Information

        public override string Android => "android";

        public override string AndroidVersion => "8.1.0";

        public override string AndroidId => "8edadfb1e4a02cdc";

        public override string AndroidBootId => "7BB3162D-F180-4e81-9065-26B01CC86F6F";

        public override string AndroidBootloader => "G9009WKEU1BOL1";

        public override string AndroidReleaseName => "REL";

        public override string AndroidIncremental => "c0ab6bb259";

        public override string AndroidInnerVersion => "lineage_kltechnduo-userdebug 8.1.0 OPM2.171026.006.H1 c0ab6bb259";

        public override string AndroidFingerprint => "samsung/klteduosctc/klte:6.0.1/MMB29M/G9009WKEU1CQB2:user/release-keys";

        #endregion

        #region Hardware Information

        public override string Baseband => "G9009WKEU1BOL1";

        public override string IMEI => "735273365111018";

        public override string IMSI => "123123333123123";

        public override ushort DisplayWidth => 1080;

        public override ushort DisplayHeight => 1920;

        public override ushort DisplayPPI => 432;

        #endregion

        #region ISP Information

        public override string ISPName => "China Telecom";

        public override string ISPApnName => "CTNET";

        #endregion

        #region WiFi Information

        public override byte[] IPAddress => new byte[] { 0x00, 0x00, 0x00, 0x00 };

        public override byte[] MACAddress => new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };

        public override byte[] BSSIDAddress => new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };

        public override string SSIDName => "<hidden SSID>";

        #endregion
    }
}
