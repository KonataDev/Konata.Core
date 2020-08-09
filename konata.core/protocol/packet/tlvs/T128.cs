using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T128 : TlvBase
    {
        private readonly bool _isNewInstall;
        private readonly bool _isGuidAvaliable;
        private readonly bool _isGuidChanged;
        private readonly byte[] _guid;
        private readonly int _guidFlag;
        private readonly string _deviceModel;
        private readonly string _deviceBrand;

        public T128(bool isNewInstall, bool isGuidAvaliable, bool isGuidChanged,
            byte[] guid, int guidFlag, string deviceModel, string deviceBrand)
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
            builder.PushInt16(0);
            builder.PushBool(_isNewInstall);
            builder.PushBool(_isGuidAvaliable);
            builder.PushBool(_isGuidChanged);
            builder.PushInt32(_guidFlag);
            builder.PushString(_deviceModel, true, true, 32);
            builder.PushBytes(_guid, false, true, true, 16);
            builder.PushString(_deviceBrand, true, true, 16);
            return builder.GetPlainBytes();
        }
    }
}
