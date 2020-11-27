using System;

namespace Konata.Runtime.Utils
{
    public static class IdGenerater
    {
        /// <summary>
        /// ID生成器初始种子
        /// </summary>
        public static long Seed { private get; set; } = 114514;

        private static DateTime basictime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static ushort tempvalue;
        /// <summary>
        /// 生成全局唯一GUID
        /// </summary>
        /// <returns>128位GUID</returns>
        public static string GenerateGUID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString("N");
        }
        
        /// <summary>
        /// 生成全局唯一ID
        /// </summary>
        /// <returns>long ID</returns>
        public static long GenerateID()
        {
            long data= (Seed << 48) + ((DateTime.UtcNow - basictime).Ticks << 16) + ++tempvalue;
            return Math.Abs(data);
        }
    }
}
