using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
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
            byte[] guid, uint guidFlag, string deviceModel, string deviceBrand) : base()
        {
            _isNewInstall = isNewInstall;
            _isGuidAvaliable = isGuidAvaliable;
            _isGuidChanged = isGuidChanged;
            _guid = guid;
            _guidFlag = guidFlag;
            _deviceModel = deviceModel;
            _deviceBrand = deviceBrand;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x128);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(0);
            PutBoolBE(_isNewInstall,1);
            PutBoolBE(_isGuidAvaliable, 1);
            PutBoolBE(_isGuidChanged, 1);
            PutUintBE(_guidFlag);
            PutString(_deviceModel,2, 32);
            PutBytes(_guid, 2, 16);
            PutString(_deviceBrand, 2, 16);
        }
    }
}
