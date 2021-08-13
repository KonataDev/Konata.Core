using System;

namespace Konata.Core.Utils
{
    /// <summary>
    /// GUID生成器
    /// </summary>
    public static class Guid
    {
        /// <summary>
        /// 生成bytes类别的GUID
        /// </summary>
        /// <returns></returns>
        public static byte[] Generate()
        {
            return new System.Guid().ToByteArray();
        }
    }
}
