using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Message.Model
{
    public class QFaceChain : MessageChain
    {
        public uint FaceId;

        public QFaceChain()
            => Type = ChainType.QFace;

        public override string ToString()
            => $"[KQ:face,id={FaceId}]";
    }
}
