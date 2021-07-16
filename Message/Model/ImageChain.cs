using System;

namespace Konata.Core.Message.Model
{
    public class ImageChain : BaseChain
    {
        public string ImageUrl { get; set; }

        public string FileName { get; set; }

        public string FileHash { get; set; }

        public ImageChain()
            => Type = ChainType.Image;

        public override string ToString()
            => $"[KQ:image,file={FileName}]";
    }
}
