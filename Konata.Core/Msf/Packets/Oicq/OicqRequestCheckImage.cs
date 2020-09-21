using System;
using Konata.Msf.Crypto;

namespace Konata.Msf.Packets.Oicq
{
    public class OicqRequestCheckImage : OicqRequest
    {
        public OicqRequestCheckImage(uint uin, KeyRing keyring)
            : base(0x0810, 0x02, uin, new XCaptcha(), 
                  keyring._shareKey, keyring._randKey, keyring._defaultPublicKey)
        {

        }
    }

    public class XCaptcha : OicqRequestBody
    {
        public XCaptcha()
        {

        }
    }
}
