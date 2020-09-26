using System;
using System.Threading;
using Konata.Msf.Network;
using Konata.Msf.Packets;
using System.Collections.Generic;

namespace Konata.Msf
{
    using SsoRequence = Int32;
    using SsoSession = UInt32;

    using SsoSeqLock = Mutex;
    using SsoSeqDict = Dictionary<string, uint>;

    internal class SsoMan
    {
        private Core _msfCore;
        private PacketMan _pakMan;

        private SsoSeqDict _ssoSeqDict;
        private SsoSeqLock _ssoSeqLock;

        private SsoRequence _ssoSequence;
        private SsoSession _ssoSession;


        internal SsoMan(Core core)
        {
            _ssoSequence = 25900;
            _ssoSession = 0x54B87ADC;

            _ssoSeqDict = new SsoSeqDict();
            _ssoSeqLock = new SsoSeqLock();

            _msfCore = core;
            _pakMan = new PacketMan(this);
        }

        /// <summary>
        /// 初始化SSO管理者並連接伺服器等待數據發送。
        /// </summary>
        /// <returns></returns>
        internal bool Initialize()
        {
            _pakMan.OpenSocket();
            return true;
        }

        /// <summary>
        /// 获取SSO序列
        /// </summary>
        /// <returns></returns>
        internal uint GetSequence()
        {
            return (uint)_ssoSequence;
        }

        /// <summary>
        /// 獲取新的SSO序列
        /// </summary>
        /// <returns></returns>
        internal uint GetNewSequence()
        {
            Interlocked.CompareExchange(ref _ssoSequence, 10000, 0x7FFFFFFF);
            return (uint)Interlocked.Add(ref _ssoSequence, 1);
        }

        /// <summary>
        /// 從服務名獲取SSO序列號, 如果沒有則會申請新的
        /// </summary>
        /// <returns></returns>
        internal uint GetServiceSequence(string name)
        {
            uint sequence;

            _ssoSeqLock.WaitOne();
            {
                if (_ssoSeqDict.ContainsKey(name))
                {
                    sequence = _ssoSeqDict[name];
                    goto ret;
                }

                sequence = GetNewSequence();
                _ssoSeqDict.Add(name, sequence);
            }

        ret:
            _ssoSeqLock.ReleaseMutex();
            return sequence;
        }

        /// <summary>
        /// 移除SSO序列號
        /// </summary>
        /// <returns></returns>
        internal void DestroyServiceSequence(string name)
        {
            _ssoSeqLock.WaitOne();
            {
                _ssoSeqDict.Remove(name);
            }
            _ssoSeqLock.ReleaseMutex();
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。本接口不會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <returns></returns>
        internal uint PostMessage(Service service, Packet packet)
        {
            return PostMessage(service, packet, GetNewSequence());
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。本接口不會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <param name="ssoSequence">SSO序列號</param>
        /// <returns></returns>
        internal uint PostMessage(Service service, Packet packet, uint ssoSequence)
        {
            var ssoMessage = new SsoMessage(ssoSequence, _ssoSession, service.name, packet);
            var toService = new ToServiceMessage(10, 2, _msfCore._uin, ssoMessage);

            _pakMan.Emit(toService);
            return ssoSequence;
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。本接口會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <returns></returns>
        internal uint SendMessage(Service service, Packet packet)
        {
            return SendMessage(service, packet, GetNewSequence());
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。本接口會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <param name="ssoSequence">SSO序列號</param>
        /// <returns></returns>
        internal uint SendMessage(Service service, Packet packet, uint ssoSequence)
        {
            return 0;
        }

        /// <summary>
        /// 阻塞等待某序號的訊息從伺服器返回。
        /// </summary>
        /// <param name="ssoSequence"></param>
        internal Packet WaitForMessage(uint ssoSequence)
        {
            return null;
        }

        /// <summary>
        /// 處理來自伺服器發送的SSO訊息, 並派遣到對應的服務路由
        /// </summary>
        /// <param name="fromService"></param>
        internal void OnFromServiceMessage(FromServiceMessage fromService)
        {
            var ssoMessage = new SsoMessage(fromService.TakeAllBytes(out byte[] _),
                _msfCore._keyRing._zeroKey);

            Console.WriteLine($"  [ssoMessage] ssoSeq => {ssoMessage._header._ssoSequence}");
            Console.WriteLine($"  [ssoMessage] ssoSession => {ssoMessage._header._ssoSession}");
            Console.WriteLine($"  [ssoMessage] ssoCommand => {ssoMessage._header._ssoCommand}");

            try
            {
                Service.Handle(_msfCore, ssoMessage._header._ssoCommand, ssoMessage._packet);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown message.\n{e.StackTrace}");
            }
        }
    }
}
