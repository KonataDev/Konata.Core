using System;
using System.Threading;
using System.Collections.Generic;
using Konata.Msf.Network;
using Konata.Msf.Packets;
using Konata.Library.IO;

namespace Konata.Msf
{
    using SsoRequence = Int32;
    using SsoSession = UInt32;

    using SsoSeqLock = Mutex;
    using SsoSeqDict = Dictionary<string, uint>;

    public class SsoMan
    {
        private Core _msfCore;
        private PacketMan _pakMan;

        private SsoSeqDict _ssoSeqDict;
        private SsoSeqLock _ssoSeqLock;

        private SsoRequence _ssoSequence;
        private SsoSession _ssoSession;

        private byte[] _d2Key;
        private byte[] _d2Token;

        private byte[] _tgtKey;
        private byte[] _tgtToken;

        public SsoMan(Core core)
        {
            _ssoSequence = 25900;
            _ssoSession = 0x54B87ADC;

            _ssoSeqDict = new SsoSeqDict();
            _ssoSeqLock = new SsoSeqLock();

            _msfCore = core;
            _pakMan = new PacketMan(this);

            _d2Token = new byte[0];
            _tgtToken = new byte[0];
        }

        /// <summary>
        /// 初始化SSO管理者並連接伺服器等待數據發送。
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return _pakMan.OpenSocket();
        }

        public bool DisConnect()
        {
            return _pakMan.CloseSocket();
        }

        /// <summary>
        /// 获取SSO序列
        /// </summary>
        /// <returns></returns>
        public uint GetSequence()
        {
            return (uint)_ssoSequence;
        }

        /// <summary>
        /// 獲取新的SSO序列
        /// </summary>
        /// <returns></returns>
        public uint GetNewSequence()
        {
            Interlocked.CompareExchange(ref _ssoSequence, 10000, 0x7FFFFFFF);
            return (uint)Interlocked.Add(ref _ssoSequence, 1);
        }

        /// <summary>
        /// 從服務名獲取SSO序列號, 如果沒有則會申請新的
        /// </summary>
        /// <returns></returns>
        public uint GetServiceSequence(string name)
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
        public void DestroyServiceSequence(string name)
        {
            _ssoSeqLock.WaitOne();
            {
                _ssoSeqDict.Remove(name);
            }
            _ssoSeqLock.ReleaseMutex();
        }

        [Obsolete]
        public void SetTgtPair(byte[] tgtToken, byte[] tgtkey)
        {
            _tgtKey = tgtkey;
            _tgtToken = tgtToken;
        }

        [Obsolete]
        public void SetD2Pair(byte[] d2Token, byte[] d2Key)
        {
            _d2Key = d2Key;
            _d2Token = d2Token;
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。本接口不會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <returns></returns>
        public uint PostMessage(Service service, ByteBuffer packet)
        {
            return PostMessage(service, packet, GetNewSequence());
        }

        /// 發送SSO訊息至伺服器。本接口不會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <param name="ssoSequence">SSO序列號</param>
        /// <returns></returns>
        public uint PostMessage(Service service, ByteBuffer packet, uint ssoSequence)
        {
            var ssoMessage = new SsoMessage(ssoSequence, _ssoSession, service.name, _tgtToken, packet);
            var toService = new ToServiceMessage(10, _msfCore.SigInfo.Uin, _d2Token, _d2Key, ssoMessage);

            _pakMan.Emit(toService);
            return ssoSequence;
        }

        /// <summary>
        /// 發送SSO訊息至伺服器。本接口會阻塞等待。
        /// </summary>
        /// <param name="service">服務名</param>
        /// <param name="packet">請求數據</param>
        /// <returns></returns>
        public uint SendMessage(Service service, Packet packet)
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
        public uint SendMessage(Service service, Packet packet, uint ssoSequence)
        {
            return 0;
        }

        /// <summary>
        /// 阻塞等待某序號的訊息從伺服器返回。
        /// </summary>
        /// <param name="ssoSequence"></param>
        public Packet WaitForMessage(uint ssoSequence)
        {
            return null;
        }

        /// <summary>
        /// 處理來自伺服器發送的SSO訊息, 並派遣到對應的服務路由
        /// </summary>
        /// <param name="fromService"></param>
        public void OnFromServiceMessage(FromServiceMessage fromService)
        {
            try
            {
                var ssoData = fromService.TakeAllBytes(out byte[] _);

                SsoMessage ssoMessage = null;
                switch (fromService._encryptType)
                {
                    case 0:
                        ssoMessage = new SsoMessage(ssoData);
                        break;
                    case 1:
                        ssoMessage = new SsoMessage(ssoData, _d2Key);
                        break;
                    case 2:
                        ssoMessage = new SsoMessage(ssoData, _msfCore.SigInfo.ZeroKey);
                        break;
                }

                Console.WriteLine($"  [ssoMessage] ssoSeq => {ssoMessage.ssoHeader.ssoSequence}");
                Console.WriteLine($"  [ssoMessage] ssoSession => {ssoMessage.ssoHeader.ssoSession}");
                Console.WriteLine($"  [ssoMessage] ssoCommand => {ssoMessage.ssoHeader.ssoCommand}");

                Service.Handle(_msfCore, ssoMessage.ssoHeader.ssoCommand, ssoMessage.ssoWupBuffer);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown message.\n{e.StackTrace}");
            }
        }
    }
}
