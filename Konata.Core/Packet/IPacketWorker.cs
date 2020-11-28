using Konata.Runtime.Base.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Packet
{
    /// <summary>
    /// 协议处理者接口
    /// </summary>
    public interface IPacketWorker
    {
        /// <summary>
        /// 将事件信息序列化为byte[]报文
        /// </summary>
        /// <param name="original"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool Serialize(KonataEventArgs original,out byte[] message);

        /// <summary>
        /// 将预翻译消息反序列化为内定消息报文
        /// </summary>
        /// <param name="original"></param>
        /// <param name="evnentpackage"></param>
        /// <returns></returns>
        bool DeSerialize(KonataEventArgs original,out KonataEventArgs evnentpackage);
    }
}
