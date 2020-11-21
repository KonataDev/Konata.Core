using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Konata.Core.MQ
{
    /// <summary>
    /// 单一消息队列实现接口
    /// </summary>
    public interface IMQ<T>
    {
        /// <summary>
        /// 标记该消息队列使用的消息包类型
        /// </summary>
        Type MsgType { get; }

        /// <summary>
        /// 标记该消息队列是否已经关闭
        /// </summary>
        bool Closed { get; }

        /// <summary>
        /// 压入数据
        /// 阻塞式
        /// </summary>
        /// <param name="data"></param>
        void Add(T data);

        /// <summary>
        /// 压入数据
        /// 阻塞式
        /// 可取消
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        void Add(T data, CancellationToken token);
        /// <summary>
        /// 尝试压入数据
        /// 非阻塞
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool TryAdd(T data,int timeout);

        /// <summary>
        /// 启动异步取出回调
        /// </summary>
        void StartTakeProcess();
        /// <summary>
        /// 关闭异步取出回调
        /// </summary>
        void StopTakeProcess();
    }
}
