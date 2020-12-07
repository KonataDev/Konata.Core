using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventAccountPullTroopList : KonataEventArgs
    {
        /// <summary>
        /// <b>[In]</b>          <br/>
        ///   Self uin.          <br/>
        /// </summary>
        public uint SelfUin { get; set; }
    }
}
