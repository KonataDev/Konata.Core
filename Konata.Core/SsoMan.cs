using System;
using System.Threading;
using System.Collections.Generic;
using Konata.Events;
using Konata.Packets;
using Konata.Packets.Sso;

namespace Konata
{
    using SsoSeqLock = Mutex;
    using SsoSeqDict = Dictionary<string, uint>;

    public class SsoMan : EventComponent
    {
        private SsoSeqDict ssoSeqDict;
        private SsoSeqLock ssoSeqLock;

        private int ssoSequence;
        private uint ssoSession;

        public SsoMan(EventPumper eventPumper)
            : base(eventPumper)
        {
            ssoSequence = 25900;
            ssoSession = 0x54B87ADC;

            ssoSeqDict = new SsoSeqDict();
            ssoSeqLock = new SsoSeqLock();
        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            if (eventParacel is EventSsoMessage ssoInfo)
                return OnPrepareNewSso(ssoInfo);
            else if (eventParacel is EventServiceMessage serviceMessage)
                return OnFromServiceMessage(serviceMessage.PayloadMsg);

            return EventParacel.Reject;
        }

        private EventParacel OnPrepareNewSso(EventSsoMessage ssoInfo)
        {
            return EventParacel.Reject;
        }

        /// <summary>
        /// 處理來自伺服器發送的SSO訊息, 並派遣到對應的服務路由
        /// </summary>
        /// <param name="fromService"></param>
        public EventParacel OnFromServiceMessage(ServiceMessage fromService)
        {
            try
            {
                var sigInfo = GetComponent<SigInfoMan>();

                var pktType = fromService.GetPacketType();
                var pktFlag = fromService.GetPacketFlag();
                var selectKey = new byte[0];
                var payloadData = new byte[0];

                switch (pktFlag)
                {
                    case RequestFlag.DefaultEmpty:
                        selectKey = null;
                        break;
                    case RequestFlag.D2Authentication:
                        selectKey = sigInfo.D2Key;
                        break;
                    case RequestFlag.WtLoginExchange:
                        selectKey = sigInfo.ZeroKey;
                        break;
                }

                payloadData = fromService.GetPayload(selectKey);
                var ssoMessage = new SsoMessage(payloadData, pktType);

                Console.WriteLine($"  [SsoMessage] ssoSeq => {ssoMessage.GetSequence()}");
                Console.WriteLine($"  [SsoMessage] ssoSession => {ssoMessage.GetSession()}");
                Console.WriteLine($"  [SsoMessage] ssoCommand => {ssoMessage.GetCommand()}");

                PostEvent<ServiceMan>(new EventSsoMessage
                {
                    RequestFlag = pktFlag,
                    PayloadMsg = ssoMessage,
                    Command = ssoMessage.GetCommand(),
                });

                return EventParacel.Accept;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown message received.");
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }

            return EventParacel.Reject;
        }

        /// <summary>
        /// 获取SSO序列
        /// </summary>
        /// <returns></returns>
        public uint GetSequence()
        {
            return (uint)ssoSequence;
        }

        /// <summary>
        /// 獲取新的SSO序列
        /// </summary>
        /// <returns></returns>
        public uint GetNewSequence()
        {
            Interlocked.CompareExchange(ref ssoSequence, 10000, 0x7FFFFFFF);
            return (uint)Interlocked.Add(ref ssoSequence, 1);
        }

        /// <summary>
        /// 從服務名獲取SSO序列號, 如果沒有則會申請新的
        /// </summary>
        /// <returns></returns>
        public uint GetServiceSequence(string name)
        {
            uint sequence;

            ssoSeqLock.WaitOne();
            {
                if (ssoSeqDict.ContainsKey(name))
                {
                    sequence = ssoSeqDict[name];
                    goto ret;
                }

                sequence = GetNewSequence();
                ssoSeqDict.Add(name, sequence);
            }

        ret:
            ssoSeqLock.ReleaseMutex();
            return sequence;
        }

        /// <summary>
        /// 移除SSO序列號
        /// </summary>
        /// <returns></returns>
        public void DestroyServiceSequence(string name)
        {
            ssoSeqLock.WaitOne();
            {
                ssoSeqDict.Remove(name);
            }
            ssoSeqLock.ReleaseMutex();
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。
        /// </summary>
        /// <param name="reqFlag"></param>
        /// <param name="reqMsg"></param>
        /// <param name="d2Token"></param>
        /// <param name="d2Key"></param>
        /// <returns></returns>
        public bool PostMessage(RequestFlag reqFlag, SsoMessage reqMsg,
            byte[] d2Token = null, byte[] d2Key = null)
        {
            //var toService = new ServiceMessage(reqFlag,
            //     reqMsg.GetPacketType(), reqMsg.GetSequence(), d2Token, d2Key)
            //    .SetUin(msfCore.SigInfo.Uin)
            //    .SetPayload(reqMsg.GetPayload());

            //return pakMan.Emit(toService);

            return false;
        }

        public uint GetSsoSession()
        {
            return ssoSession;
        }
    }
}
