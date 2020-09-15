using System;
using Konata.Msf;
using Konata.Msf.Network;
using Konata.Msf.Packets;

namespace Konata.Msf
{
    internal class SsoMan
    {
        private Core _msfCore;
        private PacketMan _pakMan;

        private uint _ssoSequence;

        private uint _ssoSession;

        internal SsoMan(Core core)
        {
            _msfCore = core;
            _pakMan = new PacketMan(this);
        }

        /// <summary>
        /// 初始化SSO管理者並連接伺服器等待數據發送。
        /// </summary>
        /// <returns></returns>
        internal bool Initialize()
        {
            _ssoSequence = 85600;
            _ssoSession = 0x01DAA2BC;

            _pakMan.OpenSocket();
            return true;
        }

        /// <summary>
        /// 獲取新的SSO序列
        /// </summary>
        /// <returns></returns>
        internal uint GetNewSequence()
        {
            return ++_ssoSequence;
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
        internal void OnSsoMessage(SsoMessage ssoMessage)
        {
            Console.WriteLine($"  [ssoMessage] ssoSeq =>\n{ssoMessage._header._ssoSequence}\n");
            Console.WriteLine($"  [ssoMessage] ssoSession =>\n{ssoMessage._header._ssoSession}\n");
            Console.WriteLine($"  [ssoMessage] ssoCommand =>\n{ssoMessage._header._ssoCommand}\n");

            _ssoSequence = ssoMessage._header._ssoSequence;
            _ssoSession = ssoMessage._header._ssoSession;

            try
            {
                ServiceRoutine.Run(_msfCore, ssoMessage._header._ssoCommand, "Handle", ssoMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown message.\n{e.StackTrace}");
            }

        }
    }
}
