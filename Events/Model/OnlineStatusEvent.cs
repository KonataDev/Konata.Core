using System;
using System.Text;

namespace Konata.Core.Events.Model
{
    public class OnlineStatusEvent : ProtocolEvent
    {
        public enum Type
        {
            Online = 11,
            Offline = 21,
            Leave = 31,
            Hidden = 41,
            Busy = 50,
            QMe = 60,
            DoNotDistrub = 70,
        }

        public enum SubType
        {
            Normal = 0,
            Offline = 255,

            BatteryPercent = 1000,
            LowSignal = 1011,
            StudyingOnline = 1024,
            TravelAtHome = 1025,
            TiMing = 1027,
            Sleeping = 1016,
            Gaming = 1017,
            Studying = 1018,
            Eating = 1019,
            Soap = 1021,
            Holiday = 1022,
            StayingUpLate = 1032,
            PlayingBall = 1050,
            FallinLove = 1051,
            IMIdle = 1052,
            ListeningMusic = 1028
        }

        /// <summary>
        /// <b>[In]</b>          <br/>
        ///   Online main type.  <br/>
        /// </summary>
        public Type EventType { get; set; }

        /// <summary>
        /// <b>[Opt] [In]</b>   <br/>
        ///   Online sub type.  <br/>
        ///     - Only valid in <b>OnlineType.Online</b>
        /// </summary>
        public SubType EventSubType { get; set; }

        /// <summary>
        /// <b>[Opt] [In]</b>   <br/>
        ///   Battery percent.  <br/>
        ///     - Only valid in OnlineType.Online with SubType.BatteryPercent
        /// </summary>
        public byte BatteryPercent { get; set; }

        /// <summary>
        /// <b>[Opt] [In]</b>   <br/>
        ///   Kick PC while login.
        /// </summary>
        public bool IsKickPC { get; set; }

        public OnlineStatusEvent()
            => WaitForResponse = true;
    }
}
