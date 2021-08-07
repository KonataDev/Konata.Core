using System;

namespace Konata.Core.Message.Model
{
    public class QFaceChain : BaseChain
    {
        public uint FaceId;

        public QFaceChain(uint face)
            : base(ChainType.QFace)
        {
            FaceId = face;
        }

        /// <summary>
        /// Create a qface chain
        /// </summary>
        /// <param name="face"></param>
        /// <param name="chain"></param>
        /// <returns></returns>
        public static QFaceChain Create(uint face)
        {
            return new(face);
        }

        /// <summary>
        /// Parse the code to a chain
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static BaseChain Parse(string code)
        {
            var args = GetArgs(code);
            {
                return Create(uint.Parse(args["id"]));
            }
        }

        public override string ToString()
            => $"[KQ:face,id={FaceId}]";
    }
}
