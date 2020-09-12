using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T128 : TlvBase
    {
        private readonly bool _isNewInstall;
        private readonly bool _isGuidAvaliable;
        private readonly bool _isGuidChanged;
        private readonly byte[] _guid;
        private readonly uint _guidFlag;
        private readonly string _deviceModel;
        private readonly string _deviceBrand;

        public T128(bool isNewInstall, bool isGuidAvaliable, bool isGuidChanged,
            byte[] guid, uint guidFlag, string deviceModel, string deviceBrand)
        {
            _isNewInstall = isNewInstall;
            _isGuidAvaliable = isGuidAvaliable;
            _isGuidChanged = isGuidChanged;
            _guid = guid;
            _guidFlag = guidFlag;
            _deviceModel = deviceModel;
            _deviceBrand = deviceBrand;
        }

        public override ushort GetTlvCmd()
        {
            return 0x128;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(0);
            builder.PutBoolBE(_isNewInstall,1);
            builder.PutBoolBE(_isGuidAvaliable, 1);
            builder.PutBoolBE(_isGuidChanged, 1);
            builder.PutUintBE(_guidFlag);
            builder.PutString(_deviceModel,2, 32);
            builder.PutBytes(_guid, 2, 16);
            builder.PutString(_deviceBrand, 2, 16);
            return builder.GetBytes();
        }
    }
}
