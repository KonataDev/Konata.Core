using System;
using Konata.Msf.Crypto;

namespace Konata.Msf.Packets.Oicq
{
    public class OicqRequestCheckSms : OicqRequest
    {
        public OicqRequestCheckSms(uint uin, KeyRing keyring)
            : base(0x0810, 0x07, uin, new XSms(),
                  keyring._shareKey, keyring._randKey, keyring._defaultPublicKey)
        {

        }
    }

    public class XSms : OicqRequestBody
    {
        public XSms()
        {

        }
    }
}
