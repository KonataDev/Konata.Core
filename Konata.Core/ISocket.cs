using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core
{
    /// <summary>
    /// 单元socket对象
    /// 用于直接操作单个socket连接情况
    /// </summary>
    public interface ISocket
    {
        bool IsAlive();

        bool StartConn();

        bool CloseConn();

    }
}
