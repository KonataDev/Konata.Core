using Konata.Model.Package;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core
{
    /// <summary>
    /// 负责socket通信类的必须接口
    /// 对于为底层通信提供链接创建-链接-中断-管理等行为
    /// </summary>
    public interface ISocketManager
    {
        /// <summary>
        /// 发起一个新的连接
        /// </summary>
        /// <returns></returns>
        bool CreateNewConnection(SocketConn conndata,out ISocket socket);
    }
}
