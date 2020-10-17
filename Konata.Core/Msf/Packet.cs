using System;
using System.IO;
using Konata.Utils;
using Konata.Library.IO;
using Konata.Msf.Utils.Crypt;
using Konata.Library.Protobuf;

namespace Konata.Msf
{
    public class Packet : ByteBuffer
    {
        public Packet()
            : base()
        {

        }

        public Packet(byte[] data)
            : base(data)
        {
            _pos = 0;

            if (data != null)
            {
                _buffer = new byte[data.Length];
                _length = (uint)data.Length;
                Buffer.BlockCopy(data, 0, _buffer, 0, data.Length);
            }
        }

        public Packet(byte[] data, ICryptor cryptor, byte[] cryptKey)
            : base()
        {
            _pos = 0;

            _buffer = cryptor.Decrypt(data, cryptKey);
            _length = (uint)_buffer.Length;
        }

        public void PutHexString(string value, Prefix prefixFlag = Prefix.None, byte limitedLength = 0)
        {
            var data = Hex.HexStr2Bytes(value);
            PutBytes(data, prefixFlag, limitedLength);
        }

        public void PutEncryptedBytes(byte[] value, ICryptor cryptor, byte[] cryptKey)
        {
            WriteData(cryptor.Encrypt(value, cryptKey));
        }

        public void PutEncryptedBytes(byte[] value, ICryptor cryptor, byte[] cryptKey,
            Prefix prefixFlag = Prefix.None, byte limitedLength = 0)
        {
            PutBytes(cryptor.Encrypt(value, cryptKey), prefixFlag, limitedLength);
        }

        /// <summary>
        /// 放入 Packet
        /// </summary>
        /// <param name="value"></param>
        public void PutPacket(Packet value)
        {
            PutBytes(value.GetBytes());
        }

        /// <summary>
        /// 加密 Packet 放入
        /// </summary>
        /// <param name="value"></param>
        public void PutPacketEncrypted(Packet value, ICryptor cryptor, byte[] cryptKey)
        {
            PutEncryptedBytes(value.GetBytes(), cryptor, cryptKey);
        }

        /// <summary>
        /// 放入 Tlv
        /// </summary>
        /// <param name="value"></param>
        public void PutTlv(Packet value)
        {
            PutBytes(value.GetBytes());
        }

        /// <summary>
        /// 放入 ProtoNode
        /// </summary>
        /// <param name="value"></param>
        public void PutProtoNode(ProtoTreeRoot value)
        {
            PutBytes(ProtoSerializer.Serialize(value));
        }

        private uint _pos;

        public string TakeHexString(out string value, Prefix prefixFlag)
        {
            return value = Hex.Bytes2HexStr(TakeBytes(out byte[] _, prefixFlag));
        }

        public byte[] TakeDecryptedBytes(out byte[] value, ICryptor cryptor, byte[] cryptKey,
            Prefix prefixFlag = Prefix.None)
        {
            return value = cryptor.Decrypt(TakeBytes(out var _, prefixFlag), cryptKey);
        }

        public byte[] TakeTlvData(out byte[] value, out ushort cmd)
        {
            if (CheckAvailable(4))
            {
                TakeUshortBE(out cmd);
                TakeUshortBE(out ushort len);
                if (CheckAvailable(len))
                {
                    value = new byte[len];
                    Buffer.BlockCopy(_buffer, (int)_pos, value, 0, len);
                    _pos += len;
                    return value;
                }
                throw new IOException("Incomplete Tlv context.");
            }
            throw new IOException("Incomplete Tlv header.");
        }

        /// <summary>
        /// 獲取打包數據並加密
        /// </summary>
        /// <returns></returns>
        public byte[] GetEncryptedBytes(ICryptor cryptor, byte[] cryptKey)
        {
            return cryptor.Encrypt(GetBytes(), cryptKey);
        }

        private uint _barExtLen;
        private uint _barPos;
        private Prefix _prefix;
        private Endian _barLenEndian;
        private bool _barEnc = false;
        private byte[] _barEncBuffer;
        private uint _barEncLength;
        private ICryptor _barEncCryptor;
        private byte[] _barEncKey;

        /// <summary>
        /// [進入屏障] 在這之後透過 PutMethods 方法組放入的數據將被計算長度
        /// </summary>
        /// <param name="prefixFlag"></param>
        /// <param name="endian"></param>
        protected void EnterBarrier(Prefix prefixFlag, Endian endian, uint extend = 0)
        {
            _barExtLen = extend;
            _barPos = _length;
            _prefix = prefixFlag;
            _barLenEndian = endian;
            PutEmpty((int)prefixFlag);
        }

        protected void EnterBarrierEncrypted(Prefix prefixFlag, Endian endian, ICryptor cryptor, byte[] cryptKey, uint extend = 0)
        {
            EnterBarrier(prefixFlag, endian, extend);
            _barEnc = true;
            _barEncBuffer = _buffer;
            _barEncLength = _length;
            _barEncCryptor = cryptor;
            _barEncKey = cryptKey;
            _buffer = null;
            _length = 0;
        }

        /// <summary>
        /// [離開屏障] 會立即在加入的數據前寫入長度
        /// </summary>
        protected void LeaveBarrier()
        {
            if (_barEnc)
            {
                byte[] enc = GetEncryptedBytes(_barEncCryptor, _barEncKey);
                _buffer = _barEncBuffer;
                _length = _barEncLength;
                PutBytes(enc);
                _barEnc = false;
                _barEncBuffer = null;
                _barEncLength = 0;
                _barEncCryptor = null;
                _barEncKey = null;
            }
            InsertPrefix(_buffer, _barPos,
                _length + _barExtLen - _barPos - (uint)_prefix, _prefix, _barLenEndian);
        }
    }
}
