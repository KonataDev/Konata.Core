using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Konata.Core.NetWork
{
    /// <summary>
    /// 单元socket对象
    /// 用于直接操作单个socket连接情况
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        bool Connected { get;}

        /// <summary>
        /// 发起连接
        /// </summary>
        /// <param name="errorcode"></param>
        /// <returns></returns>
        SocketError Connect();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendBuffer"></param>
        void Send(byte[] sendBuffer);
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        void DisConnect();

    }

    public delegate void RefBytes(ref byte[] bytes);
}
