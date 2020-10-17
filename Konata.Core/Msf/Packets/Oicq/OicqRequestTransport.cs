using System;
using System.IO;
using System.IO.Compression;
using Konata.Library.IO;
using Konata.Msf.Crypto;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Oicq
{
    public class OicqRequestTransport : OicqRequest
    {
        private const ushort OicqCommand = 0x0812;
        private const ushort OicqSubCommand = 0x0001;

        public OicqRequestTransport(uint uin, KeyRing keyring, byte[] data, bool isMsf, uint appId,
            byte[] sigSession, byte[] sigSessionKey)

            : base(OicqCommand, OicqSubCommand, uin, OicqEncryptMethod.ECDH7,
                  new XTransport(data, isMsf, appId, 85, sigSession, sigSessionKey),
                  keyring._shareKey, keyring._randKey, keyring._defaultPublicKey)
        {

        }

        public class XTransport : OicqRequestBody
        {
            public XTransport(byte[] data, bool isMsf, uint appId, uint role, byte[] sigSession,
                byte[] sigSessionKey)
                : base()
            {
                PutByte((byte)(sigSession == null ?
                    (isMsf ? 0x00 : 0x03) : (isMsf ? 0x02 : 0x01)));
                PutUshortBE((ushort)data.Length);
                PutUintBE(appId);
                PutUintBE(role);
                PutBytes(sigSession);

                PutByte(0x00);

                PutUintBE((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                PutByte(0x00);
                PutByte(0x01);
                EnterBarrierEncrypted(2, Endian.Big, TeaCryptor.Instance, sigSessionKey);
                {
                    var output = new MemoryStream();
                    var deflate = new DeflateStream(output, CompressionLevel.Fastest);
                    {
                        deflate.Write(data, 0, data.Length);
                        deflate.Close();
                    }
                    PutBytes(output.ToArray());
                }
                LeaveBarrier();
            }
        }
    }
}
