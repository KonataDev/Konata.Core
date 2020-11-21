using System;
using System.Net.Sockets;

namespace Konata.Core.NetWork
{
    /// <summary>
    /// Socket异步消息事件
    /// </summary>
    public class SocketEventHandler : SocketAsyncEventArgs
    {
        /// <summary>
        /// 消息ID
        /// 【用于消息事件复用】
        /// </summary>
        public int EventTagId { get; set; }

        /// <summary>
        /// 消息事件是否正在使用中
        /// </summary>
        public bool Using { get; set; } = false;
    }
}
