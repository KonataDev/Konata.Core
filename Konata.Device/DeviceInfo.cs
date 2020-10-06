using System;

namespace Konata.Device
{
    public abstract class DeviceInfo
    {
        #region Device Information

        public abstract string ModelName { get; }

        public abstract string Manufacturer { get; }

        #endregion

        #region Android Information

        public abstract string Android { get; }

        public abstract string AndroidVersion { get; }

        public abstract string AndroidId { get; }

        public abstract string AndroidBootId { get; }

        public abstract string AndroidBootloader { get; }

        public abstract string AndroidReleaseName { get; }

        public abstract string AndroidIncremental { get; }

        public abstract string AndroidInnerVersion { get; }

        public abstract string AndroidFingerprint { get; }

        #endregion

        #region Hardware Information

        public abstract string Baseband { get; }

        public abstract string IMEI { get; }

        public abstract string IMSI { get; }

        public abstract ushort DisplayWidth { get; }

        public abstract ushort DisplayHeight { get; }

        public abstract ushort DisplayPPI { get; }

        #endregion

        #region ISP Information

        public abstract string ISPName { get; }

        public abstract string ISPApnName { get; }

        #endregion

        #region WiFi Information

        public abstract byte[] IPAddress { get; }

        public abstract byte[] MACAddress { get; }

        public abstract byte[] BSSIDAddress { get; }

        public abstract string SSIDName { get; }

        #endregion

        public void LoadFromConfig()
        {

        }

        public void SaveToConfig()
        {

        }
    }
}
