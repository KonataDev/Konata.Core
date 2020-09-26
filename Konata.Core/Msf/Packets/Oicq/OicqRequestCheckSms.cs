using System;
using Konata.Msf.Crypto;
using Konata.Msf.Packets.Tlv;

namespace Konata.Msf.Packets.Oicq
{
    using Tlv = Tlv.Tlv;

    public class OicqRequestCheckSms : OicqRequest
    {
        private const ushort OicqCommand = 0x0810;
        private const ushort OicqSubCommand = 0x0007;

        public OicqRequestCheckSms(uint uin, KeyRing keyring)
            : base(OicqCommand, OicqSubCommand, uin, OicqEncryptMethod.ECDH7, new XSms(),
                  keyring._shareKey, keyring._randKey, keyring._defaultPublicKey)
        {

        }

        public class XSms : OicqRequestBody
        {
            public XSms()
            {
                TlvPacker tlvs = new TlvPacker();
                {
                    // tlvs.PutTlv(new Tlv(0x0008, new T8Body()));
                    // tlvs.PutTlv(new Tlv(0x0104, new T104Body(sigSission)));
                    // tlvs.PutTlv(new Tlv(0x0116, new T116Body(150470524, 66560)));
                    // tlvs.PutTlv(new Tlv(0x0174, new T174Body()));
                    // tlvs.PutTlv(new Tlv(0x017a, new T17aBody()));
                    // tlvs.PutTlv(new Tlv(0x0179, new T179Body()));
                }

                PutUshortBE(OicqSubCommand);
                PutBytes(tlvs.GetBytes(true));
            }
        }
    }
}
