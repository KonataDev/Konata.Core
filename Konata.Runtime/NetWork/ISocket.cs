using System.Net.Sockets;

namespace Konata.Runtime.Network
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
        bool Connected { get; }

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
}
