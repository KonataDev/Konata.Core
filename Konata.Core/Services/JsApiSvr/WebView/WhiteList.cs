using System;
using Konata.Events;

namespace Konata.Services.JsApiSvr.WebView
{
    public class WhiteList : ServiceRoutine
    {
        public WhiteList(EventPumper eventPumper)
            : base("JsApiSvr.webview.whitelist", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
