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

        internal static BaseChain Parse(string code)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
            => $"[KQ:face,id={FaceId}]";
    }
}
