using System;

namespace Konata.Core.Message.Model
{
    public class QFaceChain : BaseChain
    {
        public uint FaceId;

        public QFaceChain()
            => Type = ChainType.QFace;

        public QFaceChain(uint face)
        {
            FaceId = face;
            Type = ChainType.QFace;
        }
    }
}
