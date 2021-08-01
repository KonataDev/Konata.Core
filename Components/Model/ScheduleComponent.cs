using System;
using System.Collections.Generic;

using Konata.Core.Attributes;

namespace Konata.Core.Components.Model
{
    [Component("ScheduleComponent", "Konata Schedule Task Component")]
    internal class ScheduleComponent : InternalComponent
    {
        internal struct Schedule
        {
            /// <summary>
            /// Task name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Task action
            /// </summary>
            public Action Action { get; set; }

            /// <summary>
            /// Task interval
            /// </summary>
            public uint Interval { get; set; }

            /// <summary>
            /// Execute how many times
            /// Keep zero for infinite
            /// </summary>
            public uint Times { get; set; }
        }

        private List<Schedule> _taskList;

        public ScheduleComponent()
        {
            _taskList = new();

            // TODO:
            // Task thread
        }

        /// <summary>
        /// Executes the task every interval
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        public void Interval(string name, uint interval, uint times, Action action)
        {
            var task = new Schedule
            {
                Name = name,
                Action = action,
                Interval = interval,
                Times = times
            };

            // TODO:
            // Check duplicate
            _taskList.Add(task);
        }

        /// <summary>
        /// Executes the task every interval
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        public void Interval(string name, uint interval, Action action)
            => Interval(name, interval, 0, action);

        /// <summary>
        /// Run once task
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public void RunOnce(string name, uint delay, Action action)
            => Interval(name, delay, 1, action);

        /// <summary>
        /// Run once task
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="date"></param>
        /// <param name="action"></param>
        public void RunOnce(string name, DateTime date, Action action)
            => RunOnce(name, (uint)((date - DateTime.Now).TotalSeconds), action);
    }
}
