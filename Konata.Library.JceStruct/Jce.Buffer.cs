using Konata.Library.IO;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        private sealed class Buffer : ByteBuffer
        {
            public Buffer(byte[] data = null) : base(data) { }

            /// <summary>
            /// Put JCE head data.
            /// </summary>
            /// <param name="tag">Tag.</param>
            /// <param name="type">JCE type.</param>
            public void PutJceHead(byte tag, Type type)
            {
                if (tag < 0xF)
                {
                    PutByte((byte)((tag << 4) | (int)type));
                }
                else
                {
                    PutByte((byte)(0xF | (int)type));
                    PutByte(tag);
                }
            }

            /// <summary>
            /// Take JCE head data.
            /// </summary>
            /// <param name="type">JCE type.</param>
            /// <returns>Tag.</returns>
            public byte TakeJceHead(out Type type)
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
}