using Konata.Utils.IO;
using Konata.Utils.JceStruct.Model;

namespace Konata.Utils.JceStruct
{
    /// <summary>
    /// Jcez专有缓冲
    /// </summary>
    class Buffer : ByteBuffer
    {
        public Buffer(byte[] data = null) : base(data) { }

        /// <summary>
        /// JCE头报文写入
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <param name="type">JCE type.</param>
        public void TakeJceHead(byte tag, Type type)
        {
            if (tag < 0xF)
            {
                PutByte((byte)((tag << 4) | (int)type));
            }
            else
            {
                PutByte((byte)(0xF0 | (int)type));
                PutByte(tag);
            }
        }

        /// <summary>
        /// JCE头报文读取
        /// </summary>
        /// <param name="type">JCE type.</param>
        /// <returns>Tag.</returns>
        public byte PutJceHead(out Type type)
        {
            TakeByte(out byte tag);
            type = (Type)(tag & 0xF);
            tag >>= 4;
            if (tag == 0xF)
            {
                TakeByte(out tag);
            }
            return tag;
        }
    }
}
