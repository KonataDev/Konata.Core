using System;
using Konata.Core.Packet.Tlv;
using Konata.Core.Packet.Tlv.TlvModel;

namespace Konata.Core.Packet.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestCheckImage : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0002;

        public OicqRequestCheckImage(SigInfo sigInfo, string sigTicket)

            : base(OicqCommand, OicqSubCommand, sigInfo.Uin,
                  OicqEncryptMethod.ECDH7, new XCaptcha(sigInfo.WtLoginSession, sigTicket),
                  sigInfo.ShareKey, sigInfo.RandKey, sigInfo.DefaultPublicKey)
        {

        }

        public class XCaptcha : OicqRequestBody
        {
            public XCaptcha(string sigSission, string sigTicket)
                : base()
            {
                TlvPacker tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0193, new T193Body(sigTicket)));
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body()));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(sigSission)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(AppInfo.wtLoginMiscBitmap,
                        AppInfo.wtLoginSubSigBitmap, AppInfo.wtLoginSubAppIdList)));
                }

                PutUshortBE(OicqSubCommand);
                PutBytes(tlvs.GetBytes(true));
            }
        }
    }
}
