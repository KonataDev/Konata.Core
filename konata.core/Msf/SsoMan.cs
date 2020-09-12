using System;
using Konata.Msf;
using Konata.Msf.Network;
using Konata.Msf.Packets;

namespace konata.Msf
{
    internal class SsoMan
    {
        private PacketMan _pakMan;

        private uint _ssoSquence;

        private uint _ssoSession;

        internal SsoMan()
        {
            _pakMan = new PacketMan();
        }

        /// <summary>
        /// 初始化SSO管理者並連接伺服器等待數據發送。
        /// </summary>
        /// <returns></returns>
        internal bool Initialize()
        {
            _ssoSquence = 85600;
            _ssoSession = 0x01DAA2BC;

            _pakMan.OpenSocket();
            return true;
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。本接口不會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <returns></returns>
        internal uint PostMessage(Service service, Packet packet)
        {
            return _ssoSquence++;
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。本接口會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <returns></returns>
        internal uint SendMessage(Service service, Packet packet)
        {
            _pakMan.Emit(new SsoPacket(++_ssoSquence, _ssoSession, service.name, packet));
            return _ssoSquence;
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
        private void HandleSsoMessage(byte[] fromService)
        {
            // <TODO> unpack bytes and update fields and pass remain bytes to ServiceRoutine
        }
    }
}
