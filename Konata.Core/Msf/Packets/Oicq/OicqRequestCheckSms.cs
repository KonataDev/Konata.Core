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

        public OicqRequestCheckSms(uint uin, KeyRing keyring, byte[] guid, string sigSession,
            byte[] sigSecret, string sigSmsCode)

            : base(OicqCommand, OicqSubCommand, uin, OicqEncryptMethod.ECDH7,
                  new XCheckSms(sigSession, sigSecret, sigSmsCode),
                  keyring._shareKey, keyring._randKey, keyring._defaultPublicKey)
        {

        }

        public class XCheckSms : OicqRequestBody
        {
            public XCheckSms(string sigSession, byte[] sigSecret, string sigSmsCode)
            {
                TlvPacker tlvs = new TlvPacker();
                {
                    tlvs.PutTlv(new Tlv(0x0008, new T8Body(2052)));
                    tlvs.PutTlv(new Tlv(0x0104, new T104Body(sigSession)));
                    tlvs.PutTlv(new Tlv(0x0116, new T116Body(150470524, 66560)));
                    tlvs.PutTlv(new Tlv(0x0174, new T174Body(sigSecret)));
                    tlvs.PutTlv(new Tlv(0x017c, new T17cBody(sigSmsCode)));
                    tlvs.PutTlv(new Tlv(0x0198, new T198Body(1)));
                    tlvs.PutTlv(new Tlv(0x0401, new T401Body()));// G
                }

                PutUshortBE(OicqSubCommand);
                PutBytes(tlvs.GetBytes(true));
            }
        }
    }
}
