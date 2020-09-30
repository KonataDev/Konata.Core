using System;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Konata.Debug.DevToolsProtocol
{
    using CdpEventQueue = Queue<CdpEvent>;
    using CdpSequenceMap = Dictionary<uint, string>;

    public struct CdpEvent
    {
        public string _method;
        public string _data;
    }

    public class Session
    {
        private Mutex _cdpMutexEvent;
        private Mutex _cdpMutexSequence;
        private CdpEventQueue _cdpEventQueue;
        private CdpSequenceMap _cdpSequenceMap;

        private Thread _wsRecvThread;
        private ClientWebSocket _wsClient;

        private uint _sessionSequence;

        public Session(string webSocketURL)
        {
            Console.WriteLine($"Connect to {webSocketURL}");

            _wsClient = new ClientWebSocket();
            _wsClient.ConnectAsync(new Uri(webSocketURL), CancellationToken.None).Wait();

            _wsRecvThread = new Thread(new ThreadStart(ReceiveThread));
            _wsRecvThread.Start();

            _cdpMutexEvent = new Mutex();
            _cdpEventQueue = new CdpEventQueue();

            _cdpMutexSequence = new Mutex();
            _cdpSequenceMap = new CdpSequenceMap();
        }

        private void ReceiveThread()
        {
            var recvBuffer = new byte[0];
            var tokenCount = 0;
            var splitPosition = 0;

            while (true)
            {
                Thread.Sleep(0);

                byte[] segBuf = RecvSegment();
                if (segBuf.Length <= 0)
                {
                    continue;
                }

                recvBuffer = recvBuffer.Concat(segBuf).ToArray();

                if (recvBuffer.Length <= 2)
                {
                    continue;
                }

                for (int i = splitPosition; i < recvBuffer.Length; ++i)
                {
                    switch ((char)recvBuffer[i])
                    {
                        case '{': ++tokenCount; break;
                        case '}': --tokenCount; break;
                        default: break;
                    }

                    ++splitPosition;

                    if (splitPosition != 0 && tokenCount == 0)
                    {
                        var jsonBuffer = new byte[splitPosition];
                        Array.Copy(recvBuffer, jsonBuffer, splitPosition);

                        var newBuffer = new byte[recvBuffer.Length - splitPosition];
                        Array.Copy(recvBuffer, newBuffer, newBuffer.Length);
                        recvBuffer = newBuffer;
                        splitPosition = 0;

                        OnReceiveData(Encoding.UTF8.GetString(jsonBuffer));
                        break;
                    }
                }
            }
        }

        private byte[] segmentBuffer = new byte[5120];

        private byte[] RecvSegment()
        {
            _wsClient.ReceiveAsync(new ArraySegment<byte>(segmentBuffer), CancellationToken.None).Wait();

            var zeroPosition = Array.IndexOf<byte>(segmentBuffer, 0);
            if (zeroPosition <= 0)
            {
                return new byte[0];
            }

            var stringSeg = new byte[zeroPosition];
            Array.Copy(segmentBuffer, stringSeg, zeroPosition);
            Array.Clear(segmentBuffer, 0x00, zeroPosition);

            return stringSeg;
        }

        private Regex _depRegexEvent = new Regex("^{\"method\":\"([a-zA-Z.]*)\",\"params\":(.*)}$");
        private Regex _cdpRegexCallRet = new Regex("^{\"id\":([0-9]*),\"result\":(.*)}$");

        private void OnReceiveData(string json)
        {
            var matches = _depRegexEvent.Match(json);
            if (matches.Success)
            {
                OnReceiveEvent(matches.Groups[1].Value, matches.Groups[2].Value);
                return;
            }

            matches = _cdpRegexCallRet.Match(json);
            if (matches.Success)
            {
                OnReceiveCallRet(uint.Parse(matches.Groups[1].Value),
                    matches.Groups[2].Value);
                return;
            }
        }

        private void OnReceiveEvent(string method, string data)
        {
            _cdpMutexEvent.WaitOne();
            {
                _cdpEventQueue.Enqueue(new CdpEvent { _method = method, _data = data });
            }
            _cdpMutexEvent.ReleaseMutex();

            Console.WriteLine(data);
        }

        private void OnReceiveCallRet(uint sequence, string result)
        {
            _cdpMutexSequence.WaitOne();
            {
                _cdpSequenceMap.Add(sequence, result);
            }
            _cdpMutexSequence.ReleaseMutex();

            Console.WriteLine(result);
        }


        /// <summary>
        /// 啓用Network
        /// </summary>
        /// <param name="maxPostDataSize"></param>
        public void EnableNetwork(int maxPostDataSize)
        {
            CallDevTool("Network.enable(maxPostDataSize)", maxPostDataSize);
        }

        /// <summary>
        /// 啓用Page
        /// </summary>
        public void EnablePage()
        {
            CallDevTool("Page.enable()");
        }

        /// <summary>
        /// 啓用Javascript Runtime
        /// </summary>
        public void EnableRuntime()
        {
            CallDevTool("Runtime.enable()");
        }

        /// <summary>
        /// 啓用Overlay
        /// </summary>
        public void EnableOverlay()
        {
            CallDevTool("Overlay.enable()");
        }

        /// <summary>
        /// 啓用DOM
        /// </summary>
        public void EnableDOM()
        {
            CallDevTool("DOM.enable()");
        }

        /// <summary>
        /// 啓用CSS
        /// </summary>
        public void EnableCSS()
        {
            CallDevTool("CSS.enable()");
        }

        public void EnableDebugger()
        {
            CallDevTool("Debugger.enable()");
        }

        /// <summary>
        /// 啓用觸摸事件模擬
        /// </summary>
        /// <param name="maxTouchPoints"></param>
        /// <param name="touchEventType"></param>
        public void EnableTouchSimulation(int maxTouchPoints, string touchEventType)
        {
            CallDevTool("Emulation.setTouchEmulationEnabled(enabled,maxTouchPoints)",
                true, maxTouchPoints);
            CallDevTool("Emulation.setEmitTouchEventsForMouse(enabled,configuration)",
                true, touchEventType);
        }

        /// <summary>
        /// 設置額外的請求頭
        /// </summary>
        /// <param name="headers"></param>
        public void SetExtraHeaders(string[] headers)
        {
            CallDevTool("Network.setExtraHTTPHeaders(headers)", headers);
        }

        public void SetUserAgentOverride(string userAgent)
        {
            CallDevTool("Network.setUserAgentOverride(userAgent)", userAgent);
        }

        public void SetUserAgentOverride(string userAgent, string acceptLanguage)
        {
            CallDevTool("Network.setUserAgentOverride(userAgent,acceptLanguage)",
                userAgent, acceptLanguage);
        }

        public void ExecuteScript(string script)
        {
            CallDevTool("Runtime.evaluate(expression)", script);
        }

        public void DebuggerPause()
        {
            CallDevTool("Debugger.pause()");
        }

        public void DebuggerResume()
        {
            CallDevTool("Debugger.resume()");
        }

        public void SetCookie()
        {

        }

        /// <summary>
        /// 跳轉到指定連接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referrer"></param>
        public void NavigateTo(string url)
        {
            CallDevTool("Page.navigate(url)", url);
        }

        /// <summary>
        /// 跳轉到指定連接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referrer"></param>
        public void NavigateTo(string url, string referrer)
        {
            CallDevTool("Page.navigate(url,referrer)", url, referrer);
        }

        /// <summary>
        /// 等待Ticket產生
        /// </summary>
        /// <returns></returns>
        public string WaitForTicket()
        {
            return Console.ReadLine();
        }

        /// <summary>
        /// 遠端調用DevTools方法, 實質上是拼接JSON _(:3) z)_
        /// </summary>
        /// <param name="proto"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string CallDevTool(string proto, params object[] args)
        {
            var bracketFirst = proto.IndexOf("(");
            var bracketEnd = proto.IndexOf(")", bracketFirst);
            var protoMethodName = proto.Substring(0, bracketFirst);
            var protoArgCompact = proto.Substring(bracketFirst + 1, bracketEnd - bracketFirst - 1);
            var protoArgSplited = new string[0];

            if (protoArgCompact != "")
            {
                protoArgSplited = protoArgCompact.Split(',');

                if (args.Length != protoArgSplited.Length)
                {
                    return "";
                }
            }

            var callParams = new List<string>();
            for (int i = 0; i < args.Length; ++i)
            {
                string argValue = "";
                if (args[i] is bool)
                    argValue = (bool)args[i] ? "true" : "false";
                else if (args[i] is string)
                    argValue = $"\"{(string)args[i]}\"";
                else if (args[i] is int)
                    argValue = ((int)args[i]).ToString();

                callParams.Add($"\"{protoArgSplited[i]}\": {argValue}");
            }

            ++_sessionSequence;

            var json = $"{{\"id\": {_sessionSequence}, \"method\": \"{protoMethodName}\", \"params\": {{{string.Join(",", callParams)}}}}}";
            Console.WriteLine(json);

            return SendMessage(_sessionSequence, json);
        }

        /// <summary>
        /// 發送訊息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string SendMessage(uint sequence, string message)
        {
            if (_wsClient.State != WebSocketState.Open)
            {
                return "";
            }

            byte[] data;

            data = Encoding.UTF8.GetBytes(message);
            _wsClient.SendAsync(new ArraySegment<byte>(data),
                WebSocketMessageType.Text, true, CancellationToken.None).Wait(3000);

            return GetMessage(sequence);
        }

        private string GetMessage(uint sequence)
        {
            var result = "";

            while (result.Length == 0)
            {
                Thread.Sleep(0);

                _cdpMutexSequence.WaitOne();
                {
                    if (_cdpSequenceMap.ContainsKey(sequence))
                    {
                        result = _cdpSequenceMap[sequence];
                        _cdpSequenceMap.Remove(sequence);
                    }
                }
                _cdpMutexSequence.ReleaseMutex();
            }

            return result;
        }
    }
}
