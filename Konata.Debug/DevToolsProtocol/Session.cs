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
    using CdpExtraHeader = Dictionary<string, string>;

    public struct CdpEvent
    {
        public string _method;
        public string _data;
    }

    public class Session
    {
        private Mutex _cdpMutexEvent;
        private Mutex _cdpMutexCallRet;
        private CdpEventQueue _cdpEventQueue;
        private CdpSequenceMap _cdpCallRetMap;

        private Thread _wsRecvThread;
        private ClientWebSocket _wsClient;

        private uint _sessionSequence;
        private bool _sessionFinished = false;

        public Session(string webSocketURL)
        {
            Console.WriteLine($"Connect to {webSocketURL}");

            _wsClient = new ClientWebSocket();
            _wsClient.ConnectAsync(new Uri(webSocketURL), CancellationToken.None).Wait();

            _wsRecvThread = new Thread(new ThreadStart(ReceiveThread));
            _wsRecvThread.Start();

            _cdpMutexEvent = new Mutex();
            _cdpEventQueue = new CdpEventQueue();

            _cdpMutexCallRet = new Mutex();
            _cdpCallRetMap = new CdpSequenceMap();
        }

        /// <summary>
        /// 接收綫程
        /// </summary>
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

        private byte[] segmentBuffer = new byte[512000];

        /// <summary>
        /// 奇怪的Segment設定 C# nmsl
        /// </summary>
        /// <returns></returns>
        private byte[] RecvSegment()
        {
            try
            {
                _wsClient.ReceiveAsync(new ArraySegment<byte>(segmentBuffer), CancellationToken.None).Wait();
            }
            catch (Exception _)
            {
                OnDisconnect();
            }

            var zeroPosition = Array.IndexOf<byte>(segmentBuffer, 0);
            if (zeroPosition <= 0)
            {
                return new byte[0];
            }

            var stringSeg = new byte[zeroPosition];
            Array.Copy(segmentBuffer, stringSeg, zeroPosition);
            Array.Clear(segmentBuffer, 0x00, zeroPosition);

            // Console.Write(Encoding.UTF8.GetString(stringSeg));

            return stringSeg;
        }

        private readonly Regex _cdpRegexEvent =
            new Regex(@"^{""method"":""([a-zA-Z.]*)"",""params"":(.*)}$");
        private readonly Regex _cdpRegexCallRet =
            new Regex(@"^{""id"":([0-9]*),""result"":(.*)}$");
        private readonly Regex _cdpRegexRequestInfo =
            new Regex(@"""requestId"":""([0-9.]*)"".*?""url"":""(https:\/\/\S.*?)""");
        private readonly Regex _cdpRegexRequestId =
            new Regex(@"""requestId"":""([0-9.]*)""");
        private readonly Regex _cdpRegexTicket =
            new Regex(@"\\""ticket\\"":\\""([a-zA-Z0-9_*-]*)\\""");

        /// <summary>
        /// 接收到數據
        /// </summary>
        /// <param name="json"></param>
        private void OnReceiveData(string json)
        {
            var matches = _cdpRegexEvent.Match(json);
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

            Console.WriteLine("\nUnknwon: " + json);
        }

        /// <summary>
        /// 接收調試事件
        /// </summary>
        /// <param name="method"></param>
        /// <param name="data"></param>
        private void OnReceiveEvent(string method, string data)
        {
            _cdpMutexEvent.WaitOne();
            {
                _cdpEventQueue.Enqueue(new CdpEvent { _method = method, _data = data });
            }
            _cdpMutexEvent.ReleaseMutex();
        }

        /// <summary>
        /// 接收遠端調用返回值
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="result"></param>
        private void OnReceiveCallRet(uint sequence, string result)
        {
            _cdpMutexCallRet.WaitOne();
            {
                _cdpCallRetMap.Add(sequence, result);
            }
            _cdpMutexCallRet.ReleaseMutex();
        }

        /// <summary>
        /// 斷開連接
        /// </summary>
        private void OnDisconnect()
        {
            _sessionFinished = true;
            _wsRecvThread.Abort();
            _cdpMutexEvent.Dispose();
            _cdpMutexCallRet.Dispose();
            _cdpEventQueue.Clear();
            _cdpCallRetMap.Clear();
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

        /// <summary>
        /// 啓用Debugger
        /// </summary>
        public void EnableDebugger()
        {
            CallDevTool("Debugger.enable()");
        }

        /// <summary>
        /// 啓用Logger
        /// </summary>
        public void EnableLog()
        {
            CallDevTool("Log.enable()");
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
        public void SetExtraHeaders(CdpExtraHeader headers)
        {
            var headerParams = new List<string>();
            foreach (var element in headers)
            {
                headerParams.Add($"\"{element.Key}\": \"{element.Value}\"");
            }

            CallDevToolRaw("Network.setExtraHTTPHeaders()", $"{{\"headers\": {{{string.Join(",", headerParams)}}}}}");
        }

        /// <summary>
        /// 設置額外的請求頭
        /// </summary>
        public void SetExtraHeader(string headerName, string headerValue)
        {
            CallDevToolRaw("Network.setExtraHTTPHeaders()", $"{{\"headers\": {{\"{headerName}\": \"{headerValue}\"}}}}");
        }

        /// <summary>
        /// 覆蓋UserAgent
        /// </summary>
        /// <param name="userAgent"></param>
        public void SetUserAgentOverride(string userAgent)
        {
            CallDevTool("Network.setUserAgentOverride(userAgent)", userAgent);
        }

        /// <summary>
        /// 覆蓋UserAgent
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="acceptLanguage"></param>
        public void SetUserAgentOverride(string userAgent, string acceptLanguage)
        {
            CallDevTool("Network.setUserAgentOverride(userAgent,acceptLanguage)",
                userAgent, acceptLanguage);
        }

        /// <summary>
        /// 執行JavaScript脚本
        /// </summary>
        /// <param name="script"></param>
        public void ExecuteScript(string script)
        {
            CallDevTool("Runtime.evaluate(expression)", script);
        }

        /// <summary>
        /// 暫停調試器
        /// </summary>
        public void DebuggerPause()
        {
            CallDevTool("Debugger.pause()");
        }

        /// <summary>
        /// 恢復調試器
        /// </summary>
        public void DebuggerResume()
        {
            CallDevTool("Debugger.resume()");
        }

        /// <summary>
        /// 設置Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        public void SetCookie(string cookieName, string cookieValue)
        {
            CallDevTool("Network.setCookie(name,value)", cookieName, cookieValue);
        }

        /// <summary>
        /// 設置Cookie
        /// </summary>
        public void SetCookie(string cookieName, string cookieValue, string cookieDomain)
        {
            CallDevTool("Network.setCookie(name,value,domain)", cookieName, cookieValue, cookieDomain);
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
        /// 關閉瀏覽器
        /// </summary>
        public void Close()
        {
            CallDevTool("Browser.close()");
        }

        /// <summary>
        /// 獲取指定請求ID的數據
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public string GetResponseBody(string requestId)
        {
            return CallDevTool("Network.getResponseBody(requestId)", requestId);
        }

        private enum TicketRecvStatus
        {
            WaitForRequest,
            WaitForFinishLoading
        }

        /// <summary>
        /// 等待Ticket產生
        /// </summary>
        /// <returns></returns>
        public string WaitForTicket()
        {
            var ticketRequestId = "";
            var ticketStatus = TicketRecvStatus.WaitForRequest;

            while (!_sessionFinished)
            {
                Thread.Sleep(1);

                CdpEvent? e = null;
                _cdpMutexEvent.WaitOne();
                {
                    if (_cdpEventQueue.Count > 0)
                    {
                        e = _cdpEventQueue.Dequeue();
                    }
                }
                _cdpMutexEvent.ReleaseMutex();

                if (e == null)
                {
                    continue;
                }


                Console.WriteLine(e?._method + e?._data);
                switch (ticketStatus)
                {
                    case TicketRecvStatus.WaitForRequest:
                        if (e?._method == "Network.responseReceived")
                        {
                            var match = _cdpRegexRequestInfo.Match(e?._data);
                            if (match.Success)
                            {
                                var requestId = match.Groups[1].Value;
                                var requestURL = match.Groups[2].Value;

                                if (requestURL == "https://t.captcha.qq.com/cap_union_new_verify")
                                {
                                    ticketRequestId = requestId;
                                    ticketStatus = TicketRecvStatus.WaitForFinishLoading;

                                    Console.WriteLine($"Found ticket requestID => {ticketRequestId}");
                                }
                            }
                        }

                        break;
                    case TicketRecvStatus.WaitForFinishLoading:
                        if (e?._method == "Network.loadingFinished")
                        {
                            var match = _cdpRegexRequestId.Match(e?._data);
                            if (match.Success)
                            {
                                if (match.Groups[1].Value == ticketRequestId)
                                {
                                    var response = GetResponseBody(ticketRequestId);

                                    match = _cdpRegexTicket.Match(response);
                                    if (match.Success)
                                    {
                                        return match.Groups[1].Value;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return "";
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

            var json = $"{{\"id\":{_sessionSequence}, \"method\":\"{protoMethodName}\", \"params\":{{{string.Join(",", callParams)}}}}}";

            Console.WriteLine(json);

            return SendMessage(_sessionSequence, json);
        }

        /// <summary>
        /// 遠端調用DevTools方法, 實質上是拼接JSON _(:3) z)_
        /// </summary>
        /// <param name="proto"></param>
        /// <param name="rawParameters"></param>
        /// <returns></returns>
        public string CallDevToolRaw(string proto, string rawParameters)
        {
            var bracketFirst = proto.IndexOf("(");
            var protoMethodName = proto.Substring(0, bracketFirst);

            ++_sessionSequence;

            var json = $"{{\"id\":{_sessionSequence}, \"method\":\"{protoMethodName}\", \"params\":{rawParameters}}}";

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

        /// <summary>
        /// 獲取訊息
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private string GetMessage(uint sequence)
        {
            var result = "";
            var retry = 10;

            while (result.Length == 0 && --retry > 0)
            {
                Thread.Sleep(500);

                _cdpMutexCallRet.WaitOne();
                {
                    if (_cdpCallRetMap.ContainsKey(sequence))
                    {
                        result = _cdpCallRetMap[sequence];
                        _cdpCallRetMap.Remove(sequence);
                    }
                }
                _cdpMutexCallRet.ReleaseMutex();
            }

            return result;
        }
    }
}
