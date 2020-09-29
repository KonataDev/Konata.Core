using System;
using System.Threading;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Generic;

namespace Konata.Debug.DevToolsProtocol
{
    public class Session
    {
        private ClientWebSocket _wsClient;

        private uint _sessionSequence;

        public Session(string webSocketURL)
        {
            Console.WriteLine($"Connect to {webSocketURL}");

            _wsClient = new ClientWebSocket();
            _wsClient.ConnectAsync(new Uri(webSocketURL), CancellationToken.None).Wait();
        }

        public void EnableNetwork(int maxPostDataSize)
        {
            CallDevTool("Network.enable(maxPostDataSize)", maxPostDataSize);
        }

        public void EnablePage()
        {
            CallDevTool("Page.enable()");
        }

        public void EnableRuntime()
        {
            CallDevTool("Runtime.enable()");
        }

        public void EnableOverlay()
        {
            CallDevTool("Overlay.enable()");
        }

        public void EnableDOM()
        {
            CallDevTool("DOM.enable()");
        }

        public void EnableCSS()
        {
            CallDevTool("CSS.enable()");
        }

        public void EnableTouchSimulation(byte maxTouchPoints, string touchEventType)
        {
            CallDevTool("Emulation.setTouchEmulationEnabled(enabled,maxTouchPoints)",
                true, maxTouchPoints);
            CallDevTool("Emulation.setEmitTouchEventsForMouse(enabled,configuration)",
                true, touchEventType);
        }

        public void SetExtraHeaders(string[] headers)
        {
            CallDevTool("Network.setExtraHTTPHeaders(headers)", headers);
        }

        public void NavigateTo(string url)
        {
            CallDevTool("Page.navigate(url)", url);
        }

        public void NavigateTo(string url, string referrer)
        {
            CallDevTool("Page.navigate(url,referrer)", url, referrer);
        }

        public string WaitForTicket()
        {
            return "";
        }

        public string CallDevTool(string proto, params object[] args)
        {
            var bracketFirst = proto.IndexOf("(");
            var bracketEnd = proto.IndexOf(")", bracketFirst);
            var protoMethodName = proto.Substring(0, bracketFirst);
            var protoArgCompact = proto.Substring(bracketFirst + 1, bracketEnd - bracketFirst - 1);
            var protoArgSplited = protoArgCompact.Split(',');

            if (args.Length != protoArgSplited.Length)
            {
                return "";
            }

            var callParams = new List<string>();
            for (int i = 0; i < args.Length; ++i)
            {
                string argValue = "";
                if (args[i] is bool) argValue = (bool)args[i] ? "true" : "false";
                else if (args[i] is string) argValue = $"\"{(string)args[i]}\"";
                else if (args[i] is int) argValue = ((int)args[i]).ToString();

                callParams.Add($"\"{protoArgSplited[i]}\": {argValue}");
            }

            var json = $"{{\"id\": {++_sessionSequence}, \"method\": \"{protoMethodName}\", \"params\": {{{string.Join(",", callParams)}}}}}";
            return SendMessage(json);
        }

        public string SendMessage(string message)
        {
            if (_wsClient.State != WebSocketState.Open)
            {
                return "";
            }

            byte[] data;

            data = Encoding.UTF8.GetBytes(message);
            _wsClient.SendAsync(new ArraySegment<byte>(data),
                WebSocketMessageType.Text, true, CancellationToken.None).Wait(3000);

            data = new byte[1024];
            _wsClient.ReceiveAsync(new ArraySegment<byte>(data),
                CancellationToken.None).Wait(3000);

            return Encoding.UTF8.GetString(data);
        }
    }
}
