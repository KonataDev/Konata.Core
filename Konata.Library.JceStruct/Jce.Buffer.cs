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
            /// Put an integer value in minimum length.
            /// </summary>
            /// <param name="value">Value to be put.</param>
            public void PutJceIntMin(long value, byte tag = 0)
            {
                if (value == 0)
                {
                    PutJceHead(tag, Type.ZeroTag);
                }
                else if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                {
                    PutJceHead(tag, Type.Byte);
                    PutSbyte((sbyte)value);
                }
                else if (value >= short.MinValue && value <= short.MaxValue)
                {
                    PutJceHead(tag, Type.Short);
                    PutShortBE((short)value);
                }
                else if (value >= int.MinValue && value <= int.MaxValue)
                {
                    PutJceHead(tag, Type.Int);
                    PutIntBE((int)value);
                }
                else
                {
                    PutJceHead(tag, Type.Long);
                    PutLongBE(value);
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