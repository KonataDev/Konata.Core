using System;
using System.Threading;
using System.Collections.Generic;
using Konata.Msf.Network;
using Konata.Msf.Packets;
using Konata.Msf.Packets.Sso;

namespace Konata.Msf
{
    using SsoRequence = Int32;
    using SsoSession = UInt32;

    using SsoSeqLock = Mutex;
    using SsoSeqDict = Dictionary<string, uint>;

    public class SsoMan
    {
        private Core msfCore;
        private PacketMan pakMan;

        private SsoSeqDict ssoSeqDict;
        private SsoSeqLock ssoSeqLock;

        private SsoRequence ssoSequence;
        private SsoSession ssoSession;

        public SsoMan(Core core)
        {
            ssoSequence = 25900;
            ssoSession = 0x54B87ADC;

            ssoSeqDict = new SsoSeqDict();
            ssoSeqLock = new SsoSeqLock();

            msfCore = core;
            pakMan = new PacketMan(this);

        }

        /// <summary>
        /// 初始化SSO管理者並連接伺服器等待數據發送。
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return pakMan.OpenSocket();
        }

        public bool DisConnect()
        {
            return pakMan.CloseSocket();
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
            var toService = new ServiceMessage(reqFlag,
                 reqMsg.GetPacketType(), reqMsg.GetSequence(), d2Token, d2Key)
                .SetUin(msfCore.SigInfo.Uin)
                .SetPayload(reqMsg.GetPayload());

            return pakMan.Emit(toService);
        }

        public uint GetSsoSession()
        {
            return ssoSession;
        }

        /// <summary>
        /// 處理來自伺服器發送的SSO訊息, 並派遣到對應的服務路由
        /// </summary>
        /// <param name="fromService"></param>
        public void OnFromServiceMessage(ServiceMessage fromService)
        {
            try
            {
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
                        selectKey = msfCore.SigInfo.D2Key;
                        break;
                    case RequestFlag.WtLoginExchange:
                        selectKey = msfCore.SigInfo.ZeroKey;
                        break;
                }

                payloadData = fromService.GetPayload(selectKey);
                var ssoMessage = new SsoMessage(payloadData, pktType);

                Console.WriteLine($"  [ssoMessage] ssoSeq => {ssoMessage.GetSequence()}");
                Console.WriteLine($"  [ssoMessage] ssoSession => {ssoMessage.GetSession()}");
                Console.WriteLine($"  [ssoMessage] ssoCommand => {ssoMessage.GetCommand()}");

                Service.Handle(msfCore, ssoMessage.GetCommand(), ssoMessage.GetPayload());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown message.\n{e.StackTrace}");
            }
        }
    }
}
