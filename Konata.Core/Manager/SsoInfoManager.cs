using System;
using System.Collections.Generic;
using System.Text;

using Konata.Runtime.Base;

namespace Konata.Core.Manager
{
    public class SsoInfoManager : Component
    {
        /// <summary>
        /// Get sequence with auto increment
        /// </summary>
        public uint NewSequence { get; set; }

        /// <summary>
        /// Get/Set current sequence
        /// </summary>
        public uint CurrentSequence { get; set; }

        /// <summary>
        /// Get Session
        /// </summary>
        public uint Session { get; set; }
    }
}
