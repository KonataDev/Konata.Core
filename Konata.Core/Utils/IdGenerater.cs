using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Utils
{
    public static class IdGenerater
    {
        /// <summary>
        /// 生成全局唯一ID
        /// </summary>
        /// <returns></returns>
        public static string GeneraterID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString().Replace("-", "").ToUpper();
        }
    }
}
