using System;
using Konata.Msf.Crypto;
using Konata.Msf.Packets.Tlv;

namespace Konata.Msf.Packets.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestCheckImage : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0002;

        public OicqRequestCheckImage(uint uin, KeyRing keyring,
            string sigSission, string sigTicket)

            : base(OicqCommand, OicqSubCommand, uin,
                  OicqEncryptMethod.ECDH7, new XCaptcha(sigSission, sigTicket),
                  keyring._shareKey, keyring._randKey, keyring._defaultPublicKey)
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
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(150470524, 66560, AppInfo.wtLoginSubAppIdList)));
                }

                PutUshortBE(OicqSubCommand);
                PutBytes(tlvs.GetBytes(true));
            }
        }
    }
}
